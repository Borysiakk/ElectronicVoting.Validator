using System.Text.Json;
using ElectronicVoting.Validator.Application.DTO.BlockValidation;
using ElectronicVoting.Validator.Application.DTO.VoteValidation;
using ElectronicVoting.Validator.Application.Services;
using ElectronicVoting.Validator.Domain.Enums;
using ElectronicVoting.Validator.Domain.Interface;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
using ElectronicVoting.Validator.Infrastructure.Helpers;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.BlockValidation;

public class StartLocalBlockValidationCommand :ISignedCommand
{
    public Guid PendingBlockId { get; set; }
    
    public string Signature { get; set; }
    public Guid SignedByValidatorId { get; set; }    
}

public class StartLocalBlockValidationHandler(
    IPendingBlockStorageService pendingBlockStorageService,
    IVoteValidationProcessRepository voteValidationProcessRepository,
    IVoteEncryptionRepository voteEncryptionRepository,
    ILogger<StartLocalBlockValidationHandler> logger,
    IBlockRepository blockRepository,
    IPbftSequenceRepository pbftSequenceRepository)
{
    public async Task HandleAsync(StartLocalBlockValidationCommand command, bool isValidSigned, CancellationToken ct)
    {
        var originalPendingBlockDetailsResult = await pendingBlockStorageService.GetPendingBlockDetailsAsync(command.PendingBlockId, ct);
        var validatePendingBlockResult = await ValidatePendingBlock(command, originalPendingBlockDetailsResult.Value, ct);
        if (validatePendingBlockResult.IsFailed)
        {
            logger.LogError(validatePendingBlockResult.Errors.First().Message);
            return;
        }
        
        var localPendingBlockResult = await BuildPendingBlockDetails(originalPendingBlockDetailsResult.Value, ct);
        if (localPendingBlockResult.Value.Hash != originalPendingBlockDetailsResult.Value.Hash)
        {
            logger.LogError("Pending block hash is not equal to original pending block hash");
            return;
        }
        
        logger.LogDebug("Pending block is valid");
    }

    private async Task<Result> ValidatePendingBlock(StartLocalBlockValidationCommand command, PendingBlockDetailsDto pendingBlockDetails, CancellationToken ct)
    {
        if(command.SignedByValidatorId != pendingBlockDetails.ValidatorNodeId)
            return Result.Fail("Pending block id is not equal to validator node id");

        var voteEncryptionIs = pendingBlockDetails.PendingTransactions.Select(a=>a.VoteEncryption.Id).ToList();
        var voteValidationProcessIds = pendingBlockDetails.PendingTransactions.Select(a => a.VoteValidationProcess.Id).ToList();

        var voteEncryptionEntities = await voteEncryptionRepository.GetByIdsAsync(voteEncryptionIs, ct);
        var voteValidationProcessEntities = await voteValidationProcessRepository.GetByIdsAsync(voteValidationProcessIds, ct);
        
        if(voteEncryptionEntities.Count() != voteEncryptionIs.Count)
            return Result.Fail("Vote encryption is not found");
        
        if(voteValidationProcessEntities.Count() != voteValidationProcessIds.Count)
            return Result.Fail("Vote validation process is not found");

        //var areAllVoteValidationProcessesProcessedToCommit = voteValidationProcessEntities.All(a => a.Status == VoteValidationProcessStatus.ProcessedToCommit);
        //if (!areAllVoteValidationProcessesProcessedToCommit)
            //return Result.Fail("Vote validation process is not processed to commit");
            
        
        return Result.Ok();
    }

    private async Task<Result<PendingBlockDetailsDto>> BuildPendingBlockDetails(PendingBlockDetailsDto originalPendingBlockDetails, CancellationToken ct)
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
        
        pendingBlockDetails.Hash = BlockHashCalculator.ComputeBlockHash(pendingBlockDetails);
        
        return Result.Ok(pendingBlockDetails);
    }
}