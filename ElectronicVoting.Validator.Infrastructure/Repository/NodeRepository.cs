using ElectronicVoting.Validator.Domain.Entities;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace ElectronicVoting.Validator.Infrastructure.Repository;

public interface INodeRepository: IRepository<Node>
{
    Task<Node> FindMyNodeAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Node>> FindOtherHostsAsync(CancellationToken cancellationToken = default);
}

public class NodeRepository: Repository<Node>, INodeRepository
{
    public NodeRepository(ValidatorDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Node> FindMyNodeAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(a=>a.IsCurrent, cancellationToken);
    }

    public async Task<IEnumerable<Node>> FindOtherHostsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(a => a.IsCurrent == false).ToListAsync(cancellationToken);
    }
}