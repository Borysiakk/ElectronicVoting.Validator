using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using ElectronicVoting.Validator.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;

public interface IPendingBlockRepository : IRepository<PendingBlockEntity, Guid>
{
    Task<bool> ExistsAnyPendingBlockInProcessAsync(CancellationToken cancellationToken = default);
}

public class PendingBlockRepository(ValidatorLedgerDbContext context)
    : Repository<PendingBlockEntity, ValidatorLedgerDbContext, Guid>(context), IPendingBlockRepository
{
    public async Task<bool> ExistsAnyPendingBlockInProcessAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(a =>
            a.Status == PendingBlockStatus.Created ||
            a.Status == PendingBlockStatus.Processed ||
            a.Status == PendingBlockStatus.ReadyToCommit, cancellationToken: cancellationToken);
    }
}