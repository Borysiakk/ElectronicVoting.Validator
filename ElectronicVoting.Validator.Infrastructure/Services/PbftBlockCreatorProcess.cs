using ElectronicVoting.Validator.Domain.Commands;
using ElectronicVoting.Validator.Domain.Models.BlockValidation;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
using ElectronicVoting.Validator.Infrastructure.Factories;
using Wolverine;

namespace ElectronicVoting.Validator.Infrastructure.Services;

public interface IPbftBlockCreatorProcess
{
    Task ProcessAsync(CancellationToken ct);
}

public class PbftBlockCreatorProcess(
    IMessageBus messageBus,
    IPendingBlockFactory pendingBlockFactory,
    IUnitOfWork unitOfWork,
    IPendingBlockRepository pendingBlockRepository)
    : IPbftBlockCreatorProcess
{
    public async Task ProcessAsync(CancellationToken ct)
    {
        await unitOfWork.BeginTransaction(ct);
        var pendingBlockResult = await pendingBlockFactory.TryCreatePendingBlockAsync(ct);
        if (pendingBlockResult.IsFailed)
            return;
        
        await pendingBlockRepository.AddAsync(pendingBlockResult.Value, ct);
        await unitOfWork.Commit(ct);

        LeaderInitiateBlockValidationCommand leaderInitiateBlockValidationCommand = new()
        {
            PendingBlock = new PendingBlock()
            {
                Id = pendingBlockResult.Value.Id,
                PbftSequenceNumberId = pendingBlockResult.Value.PbftSequenceNumberId,
                PendingTransactions = pendingBlockResult.Value.PendingTransactions.Select(pt => new PendingTransaction()
                {
                    VoteEncryptionId = pt.VoteEncryptionId,
                    VoteValidationProcessId = pt.VoteValidationProcessId,
                }).ToList()
            }
        };
        
        await messageBus.SendAsync(leaderInitiateBlockValidationCommand);
    }
}