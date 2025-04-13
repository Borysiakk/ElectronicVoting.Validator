using ElectronicVoting.Validator.Domain.Entities.Blockchain;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace ElectronicVoting.Validator.Infrastructure.Repository;

public interface IPendingTransactionRepository: IRepository<PendingTransaction>
{
    Task<long> GetUnprocessedTransactionCountAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<PendingTransaction>> GetUnprocessedTransactionsAsync(CancellationToken cancellationToken = default);
}

public class PendingTransactionRepository : Repository<PendingTransaction>, IPendingTransactionRepository
{
    public PendingTransactionRepository(ValidatorDbContext dbContext) : base(dbContext) { }
    public async Task<long> GetUnprocessedTransactionCountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(a=>a.IsProcessed == false).CountAsync(cancellationToken);
    }

    public async Task<IEnumerable<PendingTransaction>> GetUnprocessedTransactionsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(a => a.IsProcessed == false).ToListAsync(cancellationToken);
    }
}