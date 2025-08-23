using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using Microsoft.EntityFrameworkCore;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;


public interface IBlockValidationResultRepository: IRepository<BlockValidationResultEntity, Guid>
{
    public Task<long> CountByPendingBlockId(Guid pendingBlockId, CancellationToken cancellationToken = default);
    public Task<IReadOnlyList<BlockValidationResultEntity>> GetByPendingBlockId(Guid pendingBlockId, CancellationToken cancellationToken = default);
}

public class BlockValidationResultRepository: Repository<BlockValidationResultEntity, ValidatorLedgerDbContext, Guid>, IBlockValidationResultRepository
{
    public BlockValidationResultRepository(ValidatorLedgerDbContext context) : base(context) { }

    public async Task<long> CountByPendingBlockId(Guid pendingBlockId, CancellationToken cancellationToken = default)
    {
        return await DbSet.LongCountAsync(a => a.PendingBlockEntityId == pendingBlockId, cancellationToken);
    }

    public async Task<IReadOnlyList<BlockValidationResultEntity>> GetByPendingBlockId(Guid pendingBlockId, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(a=>a.PendingBlockEntityId == pendingBlockId).ToListAsync(cancellationToken);
    }
}