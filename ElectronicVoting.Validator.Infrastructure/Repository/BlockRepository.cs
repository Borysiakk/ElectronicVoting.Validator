using ElectronicVoting.Validator.Domain.Entities.Blockchain;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace ElectronicVoting.Validator.Infrastructure.Repository;

public interface IBlockRepository: IRepository<Block>
{
    Task<bool> IsAnyBlockExists(CancellationToken cancellationToken);
    Task<Block?> GetLastBlock(CancellationToken cancellationToken);
}

public class BlockRepository: Repository<Block>, IBlockRepository
{
    public BlockRepository(ValidatorDbContext dbContext) : base(dbContext) { }
    
    public async Task<bool> IsAnyBlockExists(CancellationToken cancellationToken)
    {
        return await DbSet.AnyAsync(cancellationToken);
    }

    public async Task<Block?> GetLastBlock(CancellationToken cancellationToken)
    {
        var maxId = await DbSet.MaxAsync(block => block.Id, cancellationToken);
        return await DbSet.FirstOrDefaultAsync(block => block.Id == maxId, cancellationToken);
    }

    public async Task<Block> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await DbSet.Include(a=>a.Transactions).FirstOrDefaultAsync(a=>a.Id == id, cancellationToken);
    }
}