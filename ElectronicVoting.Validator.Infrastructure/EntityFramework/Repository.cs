using ElectronicVoting.Validator.Domain.Entities;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.Election;
using Microsoft.EntityFrameworkCore;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework;

public interface IRepository<TEntity, TId> 
    where TEntity : Entity<TId>
{
    Task<bool> ExistByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetByIdsAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
}   

public class Repository<TEntity, TContext, TId> : IRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TContext : DbContext
    where TId : notnull, IEquatable<TId>
{
    protected readonly DbSet<TEntity> DbSet;
    protected readonly TContext Context;

    public Repository(TContext context)
    {
        Context = context;
        DbSet = Context.Set<TEntity>();
    }


    public async Task<bool> ExistByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(a => a.Id.Equals(id), cancellationToken);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        DbSet.UpdateRange(entities);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        SetCreateMetadata(entity);
        var addedEntity = await DbSet.AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return addedEntity.Entity;
    }

    public async Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {  
        return await DbSet.FirstOrDefaultAsync(a=> a.Id.Equals(id), cancellationToken);
    }

    public async Task<List<TEntity>> GetByIdsAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(a => ids.Contains(a.Id)).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }
    
    private static void SetCreateMetadata(TEntity entity)
    {
        entity.CreatedDate = DateTime.UtcNow;
    }

    private static void SetUpdateMetadata(TEntity entity)
    {
        entity.ModifiedDate = DateTime.UtcNow;
    }
}