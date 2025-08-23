using ElectronicVoting.Validator.Application.DTO.BlockValidation;
using ElectronicVoting.Validator.Application.DTO.VoteValidation;
using ElectronicVoting.Validator.Application.Services;
using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using ElectronicVoting.Validator.Domain.Enums;
using ElectronicVoting.Validator.Infrastructure.Configuration;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
using ElectronicVoting.Validator.Infrastructure.Helpers;
using FluentResults;
using Microsoft.Extensions.Options;

namespace ElectronicVoting.Validator.Application.Factories;

public interface IPendingBlockFactory
{
    Task<Result<PendingBlockDetailsDto>> TryCreatePendingBlockAsync(IReadOnlyList<VoteValidationProcessEntity> validationProcessEntities, CancellationToken ct = default);
}
public class PendingBlockFactory(
    IOptions<BlockValidationOptions> blockValidationOptions,
    IBlockRepository blockRepository,
    IPbftSequenceRepository pbftSequenceRepository,
    IElectionValidatorService electionValidatorService,
    IVoteValidationProcessRepository voteValidationProcessRepository)
    : IPendingBlockFactory
{
    private readonly BlockValidationOptions _blockValidationOptions = blockValidationOptions.Value;

    public async Task<Result<PendingBlockDetailsDto>> TryCreatePendingBlockAsync(IReadOnlyList<VoteValidationProcessEntity> validationProcessEntities, CancellationToken ct = default)
    {
        return Result.Ok(await CreatePendingBlockDetails(ct));
    }
    
    private async Task<List<VoteValidationProcessEntity>> GetVoteValidationProcessWhenReadyToCommitAsync(CancellationToken ct)
    {
        var votesValidationProcess = await voteValidationProcessRepository.GetByStatusAsync(VoteValidationProcessStatus.ReadyToCommit, ct);

        foreach (var voteValidationProcess in votesValidationProcess)
            voteValidationProcess.Status = VoteValidationProcessStatus.ProcessedToCommit;
         
        await voteValidationProcessRepository.UpdateAsync(votesValidationProcess.ToArray(), ct);
        
        return votesValidationProcess;
    }
    
    private async Task<List<PendingTransactionDetailsDto>> CreatePendingTransactionsAsync(CancellationToken ct)
    {
        var votesProcessValidation = await GetVoteValidationProcessWhenReadyToCommitAsync(ct);
        if (!votesProcessValidation.Any())
            return new List<PendingTransactionDetailsDto>();
        
        return votesProcessValidation.Select(a => new PendingTransactionDetailsDto()
        {
            Id = Guid.NewGuid(),
            VoteEncryption = new VoteEncryptionDto()
            {
                Id = a.VoteEncryptionId,
                VoteEncryption = a.VoteEncryption.VoteEncryption,
            },
            VoteValidationProcess = new VoteValidationProcessDto()
            {
                Id = a.Id,
                Status = a.Status,
                StartedAt = a.StartedAt,
                FinishedAt = a.FinishedAt,
                VoteEncryptionId = a.VoteEncryptionId,
            }
        }).ToList();
    }

    private async Task<PendingBlockDetailsDto> CreatePendingBlockDetails(CancellationToken ct = default)
    {
        var pendingTransactionDetails= await CreatePendingTransactionsAsync(ct);
        
        var lastBlock = await blockRepository.GetLastBlock(ct);
        var lastSequence = await pbftSequenceRepository.GetLastSequenceNumberAsync(ct);
        var currentValidator = await electionValidatorService.GetCurrentValidatorIdAsync(ct);
        var pbftSequence = new PbftSequenceDto()
        {
            SequenceNumber = lastSequence + 1,
        };
        
        var pendingBlockDetails = new PendingBlockDetailsDto()
        {
            Id = Guid.NewGuid(),
            PbftSequence = pbftSequence,
            PreviousHash = lastBlock == null ? "" : lastBlock.Hash,
            ValidatorNodeId = currentValidator,
            PendingTransactions = pendingTransactionDetails,
            StartedAt = DateTime.UtcNow,
            FinishedAt = DateTime.UtcNow.AddMinutes(_blockValidationOptions.BlockValidationExpiryMinutes),
        };

        pendingBlockDetails.Hash = BlockHash.ComputeBlockHash(pendingBlockDetails);
        
        return pendingBlockDetails;
    }
}