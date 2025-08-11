using ElectronicVoting.Validator.Domain.Entities.Election;
using Microsoft.EntityFrameworkCore;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.Election.Repositories;

public interface IValidatorNodeRepository: IRepository<ValidatorNode, Guid>
{
    public Task<ValidatorNode> GetLeaderAsync(CancellationToken cancellationToken = default);
    public Task<long> CountAsync(CancellationToken cancellationToken = default);
    public Task<ValidatorNode> GetByName(string name, CancellationToken cancellationToken = default);
    public Task<IEnumerable<ValidatorNode>> GetAllExceptAsync (string name, CancellationToken cancellationToken = default);
}
public class ValidatorNodeRepository(ElectionDbContext context)
    : Repository<ValidatorNode, ElectionDbContext, Guid>(context), IValidatorNodeRepository

{
    public async Task<ValidatorNode> GetLeaderAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.IsLeader, cancellationToken: cancellationToken);
    }

    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.LongCountAsync(cancellationToken: cancellationToken);
    }

    public async Task<ValidatorNode> GetByName(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.Name == name, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<ValidatorNode>> GetAllExceptAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(x => x.Name != name).ToListAsync(cancellationToken);
    }
}