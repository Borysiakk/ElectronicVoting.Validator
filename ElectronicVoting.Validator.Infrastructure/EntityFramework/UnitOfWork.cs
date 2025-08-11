using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger;
using Microsoft.EntityFrameworkCore.Storage;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework;

public interface IUnitOfWork 
{
    Task SaveChanges(CancellationToken cancellationToken);
    Task Commit(CancellationToken cancellationToken);
    Task Rollback(CancellationToken cancellationToken);
    Task BeginTransaction(CancellationToken cancellationToken);
}

public class UnitOfWork: IUnitOfWork, IDisposable 
{
    private readonly ValidatorLedgerDbContext _context;
    private IDbContextTransaction _transaction;

    public UnitOfWork(ValidatorLedgerDbContext context)
    {
        _context = context;
    }

    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task Commit(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
        await _transaction?.CommitAsync(cancellationToken)!;
    }

    public async Task Rollback(CancellationToken cancellationToken)
    {
        await _transaction?.RollbackAsync(cancellationToken)!;
    }

    public async Task BeginTransaction(CancellationToken cancellationToken)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public void Dispose()
    {
        _transaction?.Dispose();
    }
}