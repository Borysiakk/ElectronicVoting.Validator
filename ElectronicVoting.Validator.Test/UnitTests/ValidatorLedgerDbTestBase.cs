using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger;
using ElectronicVoting.Validator.Test.IntegrationTests;
using Moq;

namespace ElectronicVoting.Validator.Test.UnitTests;

public class ValidatorLedgerDbTestBase: IDisposable
{
    protected Mock<IUnitOfWork> MockUnitOfWork { get; }
    protected ValidatorLedgerDbContext DbContext { get; }
    protected RepositoriesFactory RepositoriesFactory { get; }

    public ValidatorLedgerDbTestBase()
    {
        MockUnitOfWork = new Mock<IUnitOfWork>();
        DbContext = DbContextFactory.CreateInMemory<ValidatorLedgerDbContext>();
        RepositoriesFactory = new RepositoriesFactory(DbContext);
    }
    
    public void Dispose()
    {
        DbContext.Dispose();
    }
}