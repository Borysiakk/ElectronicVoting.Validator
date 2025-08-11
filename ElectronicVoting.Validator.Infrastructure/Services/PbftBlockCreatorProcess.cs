using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using ElectronicVoting.Validator.Domain.Enums;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
using ElectronicVoting.Validator.Infrastructure.Factories;
using ElectronicVoting.Validator.Infrastructure.Helpers;

namespace ElectronicVoting.Validator.Infrastructure.Services;

public interface IPbftBlockCreatorProcess
{
    Task ProcessAsync(CancellationToken ct);
}

public class PbftBlockCreatorProcess(
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
    }
}