using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using Microsoft.EntityFrameworkCore;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;

public interface ILocalVoteValidationProcessRepository :IRepository<LocalVoteValidationProcessEntity, Guid>
{
    Task<LocalVoteValidationProcessEntity> GetByVoteValidationProcessIdAsync(Guid voteValidationProcessId, CancellationToken cancellationToken = default);
    Task<IEnumerable<LocalVoteValidationProcessEntity>> GetByVoteValidationProcessIdsAsync(IEnumerable<Guid> voteValidationProcessIds, CancellationToken cancellationToken = default);
}

public class LocalVoteValidationProcessRepository(ValidatorLedgerDbContext electionDbContext)
    : Repository<LocalVoteValidationProcessEntity, ValidatorLedgerDbContext, Guid>(electionDbContext),
        ILocalVoteValidationProcessRepository
{
    public async Task<LocalVoteValidationProcessEntity> GetByVoteValidationProcessIdAsync(Guid voteValidationProcessId, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(a => a.VoteValidationProcessId == voteValidationProcessId, cancellationToken);
    }

    public async Task<IEnumerable<LocalVoteValidationProcessEntity>> GetByVoteValidationProcessIdsAsync(IEnumerable<Guid> voteValidationProcessIds, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(a => voteValidationProcessIds
                .Contains(a.VoteValidationProcessId))
            .ToListAsync(cancellationToken);   
    }
}
