using ElectronicVoting.Validator.Domain.Entities.Blockchain;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;

namespace ElectronicVoting.Validator.Infrastructure.Repository;

public interface ITransactionRepository: IRepository<Transaction>
{
    
}

public class TransactionRepository: Repository<Transaction>, ITransactionRepository
{
    public TransactionRepository(ValidatorDbContext dbContext) : base(dbContext) { }
}