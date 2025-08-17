using ElectronicVoting.Validator.Application.DTO.BlockValidation;
using ElectronicVoting.Validator.Application.Handlers.Commands.BlockValidation;
using ElectronicVoting.Validator.Domain.Interface.Processes;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
using ElectronicVoting.Validator.Infrastructure.Factories;
using Wolverine;

namespace ElectronicVoting.Validator.Application.Processes;

public class PbftBlockCreatorProcess(
    IUnitOfWork unitOfWork,
    IMessageBus messageBus,
    IPendingBlockFactory pendingBlockFactory,
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
            PendingBlock = new PendingBlockDto()
            {
                Id = pendingBlockResult.Value.Id,
                PbftSequence = new PbftSequenceDto()
                {
                    Id = pendingBlockResult.Value.PbftSequence.Id,
                    SequenceNumber = pendingBlockResult.Value.PbftSequence.SequenceNumber,
                },
                PendingTransactions = pendingBlockResult.Value.PendingTransactions.Select(pt => new PendingTransactionDto()
                {
                    VoteEncryptionId = pt.VoteEncryptionId,
                    VoteValidationProcessId = pt.VoteValidationProcessId,
                }).ToList()
            }
        };
        
        await messageBus.SendAsync(leaderInitiateBlockValidationCommand);
    }
}