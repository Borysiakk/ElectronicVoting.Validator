using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;

public interface IPendingTransactionRepository : IRepository<PendingTransactionEntity, Guid>;

public class PendingTransactionRepository(ValidatorLedgerDbContext context)
    : Repository<PendingTransactionEntity, ValidatorLedgerDbContext, Guid>(context), IPendingTransactionRepository;