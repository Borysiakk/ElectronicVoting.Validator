using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.Election;
using Microsoft.EntityFrameworkCore;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;

public interface IVoteConsensusConfirmationRepository: IRepository<VoteConsensusConfirmationEntity, Guid>
{
    Task<IEnumerable<VoteConsensusConfirmationEntity>> GetByVoteValidationProcessIdAsync(Guid voteValidationProcessId, CancellationToken cancellationToken = default);
    Task<long> CountByVoteValidationProcessId(Guid voteValidationProcessId, CancellationToken cancellationToken = default);
}

public class VoteConsensusConfirmationRepository(ValidatorLedgerDbContext electionDbContext)
    : Repository<VoteConsensusConfirmationEntity, ValidatorLedgerDbContext, Guid>(electionDbContext),
        IVoteConsensusConfirmationRepository
{
    public async Task<IEnumerable<VoteConsensusConfirmationEntity>> GetByVoteValidationProcessIdAsync(Guid voteValidationProcessId, CancellationToken cancellationToken = default)
    {
        return await Context.VoteConsensusConfirmation.Where(a => a.VoteValidationProcessId == voteValidationProcessId)
            .ToListAsync(cancellationToken);
    }

    public async Task<long> CountByVoteValidationProcessId(Guid voteValidationProcessId, CancellationToken cancellationToken = default)
    {
        return await Context.VoteConsensusConfirmation.CountAsync(a => a.VoteValidationProcessId == voteValidationProcessId, cancellationToken);   
    }
}