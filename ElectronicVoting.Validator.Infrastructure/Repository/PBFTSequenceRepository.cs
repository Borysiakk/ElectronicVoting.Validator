using ElectronicVoting.Validator.Domain.Entities.Blockchain;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace ElectronicVoting.Validator.Infrastructure.Repository;

public interface IPBFTSequenceRepository: IRepository<PBFTSequence>
{
    Task<PBFTSequence> GetLastSequenceAsync(CancellationToken cancellationToken = default);
    Task<long> GetLastSequenceNumberAsync(CancellationToken cancellationToken = default);
}

public class PBFTSequenceRepository: Repository<PBFTSequence>, IPBFTSequenceRepository
{
    public PBFTSequenceRepository(ValidatorDbContext dbContext) : base(dbContext) { }

    public async Task<PBFTSequence> GetLastSequenceAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(seq => seq.Id == DbSet.Max(seq => seq.Id))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<long> GetLastSequenceNumberAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(seq => seq.Id == DbSet.Max(seq => seq.Id))
            .Select(seq => seq.SequenceNumber)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
