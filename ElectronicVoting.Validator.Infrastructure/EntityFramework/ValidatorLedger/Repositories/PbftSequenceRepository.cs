using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using Microsoft.EntityFrameworkCore;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;

public interface IPbftSequenceRepository: IRepository<PbftSequenceEntity, long>
{
    Task<long> GetLastSequenceNumberAsync(CancellationToken cancellationToken = default);
    Task<PbftSequenceEntity> GetLastSequenceAsync(CancellationToken cancellationToken = default);
}

public class PbftSequenceRepository(ValidatorLedgerDbContext context): Repository<PbftSequenceEntity, ValidatorLedgerDbContext, long>(context), IPbftSequenceRepository
{
    public async Task<PbftSequenceEntity> GetLastSequenceAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(seq => seq.Id == DbSet.Max(x => x.Id)).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<long> GetLastSequenceNumberAsync(CancellationToken cancellationToken = default)
    {
        var max = await DbSet.MaxAsync(a => (long?)a.SequenceNumber, cancellationToken);
        return max ?? 0L;
    }
}