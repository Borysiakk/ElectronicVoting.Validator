using EFCore.BulkExtensions;
using ElectronicVoting.Validator.Domain.Entities;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace ElectronicVoting.Validator.Infrastructure.Repository;

public interface IRepository<T> where T : Entity
{
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<long>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task RemoveAsync(T entity, CancellationToken cancellationToken = default);

    Task<T> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<long> CountAsync(CancellationToken cancellationToken = default);
    Task<long> GetLastIdAsync(CancellationToken cancellationToken = default);
}

public class Repository<T> : IRepository<T> where T : Entity
{
    protected readonly DbSet<T> DbSet;
    protected readonly ValidatorDbContext DbContext;
    
    public Repository(ValidatorDbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = DbContext.Set<T>();
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        SetCreateMetadata(entity);
        var addedEntity = await DbSet.AddAsync(entity, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken); 
        return addedEntity.Entity;
    }

    public async Task<IEnumerable<long>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await DbContext.BulkInsertAsync<T>(entities, new BulkConfig
        {
            SetOutputIdentity = true
        }, cancellationToken: cancellationToken);
            
        return entities.Select(x => x.Id);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        SetUpdateMetadata(entity);
        DbContext.Update(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
            SetUpdateMetadata(entity);
            
        await DbContext.BulkUpdateAsync<T>(entities,  new BulkConfig
        {
            SetOutputIdentity = true
        }, cancellationToken: cancellationToken);
            
        await DbContext.SaveChangesAsync(cancellationToken); 
    }

    public async Task RemoveAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        await DbContext.SaveChangesAsync(cancellationToken); 
    }

    public async Task<T> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken); 
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.LongCountAsync(cancellationToken);
    }

    public async Task<long> GetLastIdAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(cancellationToken)
            ? await DbSet.MaxAsync(x => x.Id, cancellationToken)
            : 0L;

    }

    private static void SetCreateMetadata(T entity)
    {
        entity.CreatedDate = DateTime.UtcNow;
    }

    private static void SetUpdateMetadata(T entity)
    {
        entity.ModifiedDate = DateTime.UtcNow;
    }
}