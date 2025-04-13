using Microsoft.EntityFrameworkCore.Storage;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework;

public interface IUnitOfWork
{
    Task SaveChanges(CancellationToken cancellationToken);
    Task Commit(CancellationToken cancellationToken);
    Task Rollback(CancellationToken cancellationToken);
    Task BeginTransaction(CancellationToken cancellationToken);
}

public class UnitOfWork : IUnitOfWork, IDisposable
{
    protected IDbContextTransaction Transaction;
    protected readonly ValidatorDbContext DatabaseContext;

    public UnitOfWork(ValidatorDbContext databaseContext)
    {
        DatabaseContext = databaseContext;
    }

    public async Task BeginTransaction(CancellationToken cancellationToken)
    {
        if (Transaction != null)
            throw new InvalidOperationException("Transaction is already started");
        
        Transaction = await DatabaseContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task Commit(CancellationToken cancellationToken)
    {
        if (Transaction == null)
            throw new InvalidOperationException("Transaction is not created");

        await DatabaseContext.SaveChangesAsync(cancellationToken);
        await Transaction.CommitAsync(cancellationToken);
    }

    public async Task Rollback(CancellationToken cancellationToken)
    {
        if (Transaction == null)
            throw new InvalidOperationException("Transaction is not created");

        await Transaction.RollbackAsync(cancellationToken);
    }

    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        await DatabaseContext.SaveChangesAsync(cancellationToken);
    }
    
    public void Dispose()
    {
        Transaction?.Dispose();
        Transaction = null;
    }
}