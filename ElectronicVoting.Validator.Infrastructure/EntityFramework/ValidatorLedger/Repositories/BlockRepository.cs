using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using Microsoft.EntityFrameworkCore;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;

public interface IBlockRepository: IRepository<BlockEntity, long>
{
    Task<bool> IsAnyBlockExists(CancellationToken cancellationToken);
    Task<BlockEntity> GetLastBlock(CancellationToken cancellationToken);
}

public class BlockRepository(ValidatorLedgerDbContext context)
    : Repository<BlockEntity, ValidatorLedgerDbContext, long>(context), IBlockRepository
{
    public async Task<bool> IsAnyBlockExists(CancellationToken cancellationToken)
    {
        return await DbSet.AnyAsync(cancellationToken);
    }

    public async Task<BlockEntity> GetLastBlock(CancellationToken cancellationToken)
    {
        return await DbSet
            .OrderByDescending(block => block.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}