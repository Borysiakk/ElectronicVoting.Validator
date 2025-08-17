using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using ElectronicVoting.Validator.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;

public interface IVoteValidationProcessRepository :IRepository<VoteValidationProcessEntity, Guid>
{

    Task<VoteValidationProcessEntity> GetByVoteEncryptionIdAsync(Guid voteEncryptionId, CancellationToken cancellationToken = default);
    Task<List<VoteValidationProcessEntity>> GetByStatusAsync(VoteValidationProcessStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VoteValidationProcessEntity>> GetReadyForTimeoutAsync(DateTime dateTime, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VoteValidationProcessEntity>> GetByVoteEncryptionIdsAsync(IEnumerable<Guid> voteEncryptionIds, CancellationToken cancellationToken = default);
}

public class VoteValidationProcessRepository(ValidatorLedgerDbContext electionDbContext)
    : Repository<VoteValidationProcessEntity, ValidatorLedgerDbContext, Guid>(electionDbContext),
        IVoteValidationProcessRepository
{
    
    public async Task<VoteValidationProcessEntity> GetByVoteEncryptionIdAsync(Guid voteEncryptionId, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(a => a.VoteEncryptionId == voteEncryptionId, cancellationToken);
    }

    public async Task<List<VoteValidationProcessEntity>> GetByStatusAsync(VoteValidationProcessStatus status, CancellationToken cancellationToken = default)
    {
        return await DbSet.Include(vvp => vvp.VoteEncryption).Where(a => a.Status == status).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<VoteValidationProcessEntity>> GetReadyForTimeoutAsync(DateTime dateTime, CancellationToken cancellationToken = default)
    {
        return await Context.VoteValidationProcesses
                .Where(a =>
                    (a.Status == VoteValidationProcessStatus.Registered || a.Status == VoteValidationProcessStatus.InProgress ) &&
                    a.FinishedAt <= dateTime).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<VoteValidationProcessEntity>> GetByVoteEncryptionIdsAsync(IEnumerable<Guid> voteEncryptionIds, CancellationToken cancellationToken = default)
    {
        return await DbSet.Include(vvp => vvp.VoteEncryption)
            .Where(a => voteEncryptionIds.Contains(a.VoteEncryptionId))
            .ToListAsync(cancellationToken);   
    }
}