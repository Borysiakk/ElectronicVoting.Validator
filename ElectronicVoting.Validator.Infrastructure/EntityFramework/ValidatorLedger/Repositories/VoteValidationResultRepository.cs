using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using Microsoft.EntityFrameworkCore;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;

public interface IVoteValidationResultRepository :IRepository<VoteValidationResultEntity, Guid>
{
    Task<long> CountResultsForLocalValidationProcessAsync(Guid localVoteValidationProcessId, CancellationToken cancellationToken = default);
    Task<bool> ExistsVoteValidationResultAsync(Guid voteValidationProcessId, Guid validatorId, CancellationToken cancellationToken = default);
}

public class VoteValidationResultRepository(ValidatorLedgerDbContext context)
    : Repository<VoteValidationResultEntity, ValidatorLedgerDbContext, Guid>(context), IVoteValidationResultRepository
{
    public async Task<long> CountResultsForLocalValidationProcessAsync(Guid localVoteValidationProcessId, CancellationToken cancellationToken = default)
    {
        return await DbSet.LongCountAsync(a => a.LocalVoteValidationProcessId == localVoteValidationProcessId,
            cancellationToken);
    }

    public async Task<bool> ExistsVoteValidationResultAsync(Guid voteValidationProcessId, Guid validatorId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(a=>a.VoteValidationProcessId == voteValidationProcessId && a.ValidatorId == validatorId, cancellationToken);
    }
}

    