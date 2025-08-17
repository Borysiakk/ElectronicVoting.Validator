using ElectronicVoting.Validator.Application.Factories;
using ElectronicVoting.Validator.Application.Handlers.Commands.BlockValidation;
using ElectronicVoting.Validator.Domain.Interface.Processes;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
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
        var pendingBlockDetailsResult = await pendingBlockFactory.TryCreatePendingBlockAsync(ct);
        if (pendingBlockDetailsResult.IsFailed)
            return;
        
        LeaderInitiateBlockValidationCommand leaderInitiateBlockValidationCommand = new()
        {
            PendingBlockDetails = pendingBlockDetailsResult.Value,
        };
        
        await messageBus.SendAsync(leaderInitiateBlockValidationCommand);
    }
}