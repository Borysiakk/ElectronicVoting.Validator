using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using ElectronicVoting.Validator.Domain.Enums;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
using ElectronicVoting.Validator.Infrastructure.Helpers;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace ElectronicVoting.Validator.Infrastructure.Factories;

public interface IPendingBlockFactory
{
    Task<Result<PendingBlockEntity>> TryCreatePendingBlockAsync(CancellationToken ct = default);
}

public class PendingBlockFactory(
    IVoteValidationProcessRepository voteValidationProcessRepository,
    IPendingBlockRepository pendingBlockRepository,
    ILogger<PendingBlockFactory> logger,
    IBlockRepository blockRepository,
    IPbftSequenceRepository pbftSequenceRepository)
    : IPendingBlockFactory
{
    public async Task<Result<PendingBlockEntity>> TryCreatePendingBlockAsync(CancellationToken ct = default)
    {
        var isAnyPendingBlockInProcess = await pendingBlockRepository.ExistsAnyPendingBlockInProcessAsync(ct);
        if (isAnyPendingBlockInProcess)
        {
            logger.LogWarning("There is already a pending block in process");
            return Result.Fail("There is already a pending block in process");   
        }

        var voteValidationsReadyToCommit= await voteValidationProcessRepository.GetByStatusAsync(VoteValidationProcessStatus.ReadyToCommit, ct);
        if (!voteValidationsReadyToCommit.Any())
        {
            logger.LogWarning("There is no vote validation ready to commit");
            return Result.Fail("There is no vote validation ready to commit");   
        }

        var pendingTransactions= await CreatePendingTransactionsAsync(ct);
        var pendingBlock = await CreatePendingBlock(pendingTransactions, ct);
        
        return Result.Ok(pendingBlock);
    }
    
    private async Task<List<VoteValidationProcessEntity>> GetVoteValidationProcessWhenReadyToCommitAsync(CancellationToken ct)
    {
        var votesValidationProcess = await voteValidationProcessRepository.GetByStatusAsync(VoteValidationProcessStatus.ReadyToCommit, ct);

        foreach (var voteValidationProcess in votesValidationProcess)
            voteValidationProcess.Status = VoteValidationProcessStatus.ProcessedToCommit;
        
        return votesValidationProcess;
    }
    
    private async Task<List<PendingTransactionEntity>> CreatePendingTransactionsAsync(CancellationToken ct)
    {
        var votesProcessValidation = await GetVoteValidationProcessWhenReadyToCommitAsync(ct);
        return votesProcessValidation.Select(a => new PendingTransactionEntity()
        {
            Id = Guid.NewGuid(),
            VoteValidationProcessId = a.Id,
            VoteEncryptionId = a.VoteEncryptionId,
        }).ToList();
    }
    
    private async Task<PendingBlockEntity> CreatePendingBlock(List<PendingTransactionEntity> pendingTransactions, CancellationToken ct = default)
    {
        var lastBlock = await blockRepository.GetLastBlock(ct);
        var lastSequence = await pbftSequenceRepository.GetLastSequenceNumberAsync(ct);

        var pbftSequenceEntity = new PbftSequenceEntity()
        {
            SequenceNumber = lastSequence + 1,
        };
        
        var pendingBlockEntity = new PendingBlockEntity()
        {
            Id = Guid.NewGuid(),
            StartedAt = DateTime.UtcNow,
            PbftSequence = pbftSequenceEntity,
            Status = PendingBlockStatus.Created,
            PendingTransactions = pendingTransactions,
            PreviousHash = lastBlock == null ? "" : lastBlock.Hash, 
        };
        
        pendingBlockEntity.Hash = BlockHashCalculator.ComputeBlockHash(pendingBlockEntity);
        logger.LogInformation("Created pending block");
        return pendingBlockEntity;
    }
    
}