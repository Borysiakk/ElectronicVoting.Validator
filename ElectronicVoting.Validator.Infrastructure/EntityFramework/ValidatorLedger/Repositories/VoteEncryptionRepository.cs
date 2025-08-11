using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using Microsoft.EntityFrameworkCore;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;

public interface IVoteEncryptionRepository :IRepository<VoteEncryptionEntity, Guid>
{
    Task<bool> AnyExistsAsync(Guid id, CancellationToken cancellationToken = default);
}

public class VoteEncryptionRepository(ValidatorLedgerDbContext electionDbContext)
    : Repository<VoteEncryptionEntity, ValidatorLedgerDbContext, Guid>(electionDbContext), IVoteEncryptionRepository
{
    public async Task<bool> AnyExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(a=> a.Id.Equals(id), cancellationToken);
    }
}