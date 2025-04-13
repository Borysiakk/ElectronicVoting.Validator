using ElectronicVoting.Validator.Domain.Entities.Blockchain;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;

namespace ElectronicVoting.Validator.Infrastructure.Repository;

public interface IBlockConfirmationRepository: IRepository<BlockConfirmation>
{
    
}

public class BlockConfirmationRepository: Repository<BlockConfirmation>, IBlockConfirmationRepository
{
    public BlockConfirmationRepository(ValidatorDbContext dbContext) : base(dbContext)
    {
    }
}