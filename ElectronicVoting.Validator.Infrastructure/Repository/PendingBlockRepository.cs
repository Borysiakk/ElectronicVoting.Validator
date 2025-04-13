using ElectronicVoting.Validator.Domain.Entities.Blockchain;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace ElectronicVoting.Validator.Infrastructure.Repository;

public interface IPendingBlockRepository : IRepository<PendingBlock>
{
    public Task<bool> IsLastBlockProcessed(CancellationToken cancellationToken);
}

public class PendingBlockRepository: Repository<PendingBlock>, IPendingBlockRepository
{
    public PendingBlockRepository(ValidatorDbContext dbContext) : base(dbContext) { }
    
    public async Task<bool> IsLastBlockProcessed(CancellationToken cancellationToken)
    {
        return await DbSet.Where(bk => bk.Id == DbSet.Max(bk => bk.Id))
                          .Select(bk => bk.IsProcessed)
                          .FirstOrDefaultAsync(cancellationToken);
    }
}