using ElectronicVoting.Validator.Application.DTO.BlockValidation;
using ElectronicVoting.Validator.Application.DTO.VoteValidation;
using ElectronicVoting.Validator.Domain.Enums;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
using ElectronicVoting.Validator.Infrastructure.Helpers;
using FluentResults;

namespace ElectronicVoting.Validator.Application.Processes;

public interface IPbftBlockValidationProcess
{
    public Task<Result<PendingBlockDetailsDto>> ProcessAsync(PendingBlockDetailsDto pendingBlockDetailsDto, Guid signedByValidatorId, CancellationToken ct);
}

public class PbftBlockValidationProcess(
    IBlockRepository blockRepository,
    IPbftSequenceRepository pbftSequenceRepository,
    IVoteEncryptionRepository voteEncryptionRepository,
    IVoteValidationProcessRepository voteValidationProcessRepository)
    : IPbftBlockValidationProcess
{
    
    public async Task<Result<PendingBlockDetailsDto>> ProcessAsync(PendingBlockDetailsDto pendingBlockDetailsDto, Guid signedByValidatorId, CancellationToken ct)
    {
        var canReconstructPendingBlock = await CanReconstructPendingBlock(pendingBlockDetailsDto, signedByValidatorId, ct);
        if (canReconstructPendingBlock.IsFailed)
            return Result.Fail<PendingBlockDetailsDto>(canReconstructPendingBlock.Errors);
        
        return await ReconstructPendingBlockDetails(pendingBlockDetailsDto, ct);
    }

    private async Task<Result> CanReconstructPendingBlock(PendingBlockDetailsDto pendingBlockDetails, Guid signedByValidatorId, CancellationToken ct)
    {
        if(signedByValidatorId != pendingBlockDetails.ValidatorNodeId)
            return Result.Fail("Pending block id is not equal to validator node id");
        
        var voteEncryptionIs = pendingBlockDetails.PendingTransactions.Select(a=>a.VoteEncryption.Id).ToList();
        var voteValidationProcessIds = pendingBlockDetails.PendingTransactions.Select(a => a.VoteValidationProcess.Id).ToList();

        var voteEncryptionEntities = await voteEncryptionRepository.GetByIdsAsync(voteEncryptionIs, ct);
        var voteValidationProcessEntities = await voteValidationProcessRepository.GetByIdsAsync(voteValidationProcessIds, ct);
        
        if(voteEncryptionEntities.Count() != voteEncryptionIs.Count)
            return Result.Fail("Vote encryption is not found");
        
        if(voteValidationProcessEntities.Count() != voteValidationProcessIds.Count)
            return Result.Fail("Vote validation process is not found");
        
        return Result.Ok();
    }

    private async Task<Result<PendingBlockDetailsDto>> ReconstructPendingBlockDetails(PendingBlockDetailsDto originalPendingBlockDetails, CancellationToken ct)
    {
        var lastBlock = await blockRepository.GetLastBlock(ct);
        var lastSequence = await pbftSequenceRepository.GetLastSequenceNumberAsync(ct);
        
        var voteEncryptionIds = originalPendingBlockDetails.PendingTransactions.Select(a=>a.VoteEncryption.Id).ToList();
        var voteValidationProcessEntities = await voteValidationProcessRepository.GetByVoteEncryptionIdsAsync(voteEncryptionIds, ct);

        PbftSequenceDto pbftSequence = new()
        {
            Id = originalPendingBlockDetails.PbftSequence.Id,
            SequenceNumber = lastSequence + 1,
        };
        
        var pendingTransactions = voteValidationProcessEntities.Select(a => new PendingTransactionDetailsDto()
        {
            Id = originalPendingBlockDetails.PendingTransactions.First(b=>b.VoteValidationProcess.Id == a.Id).Id,
            VoteEncryption = new VoteEncryptionDto()
            {
                Id = a.VoteEncryptionId,
                VoteEncryption = a.VoteEncryption.VoteEncryption,
            },
            VoteValidationProcess = new VoteValidationProcessDto()
            {
                Id = a.Id,
                Status = VoteValidationProcessStatus.ProcessedToCommit, // Do zmiany potem bo narazie ten status nie zostal zaktulizowany, trzeba dopisac aktulaizacje statusu (chyba bo pamietam ze ustawiam ten status)
                StartedAt = a.StartedAt,
                FinishedAt = a.FinishedAt,
                VoteEncryptionId = a.VoteEncryptionId
            }
        }).ToList();

        PendingBlockDetailsDto pendingBlockDetails = new()
        {
            PbftSequence = pbftSequence,
            Id = originalPendingBlockDetails.Id,
            PendingTransactions = pendingTransactions,
            PreviousHash = lastBlock == null ? "" : lastBlock.Hash,
            ValidatorNodeId = originalPendingBlockDetails.ValidatorNodeId,
        };
        
        pendingBlockDetails.Hash = BlockHash.ComputeBlockHash(pendingBlockDetails);
        
        return Result.Ok(pendingBlockDetails);
    }
}