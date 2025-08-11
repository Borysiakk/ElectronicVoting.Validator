using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
using ElectronicVoting.Validator.Test.CertificateTools;
using Xunit;

namespace ElectronicVoting.Validator.Test.IntegrationTests;

[Collection("ValidatorFactory")]
public class VoteValidatedTest
{
    private readonly ValidatorFactory _factory;
    
    public VoteValidatedTest(ValidatorFactory factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task Test1()
    {
        await _factory.InitializeAsync();
        var validatorInstances = new List<ValidatorInstance>()
        {
            await _factory.Create("electronicvoting.validator_a", true),
            await _factory.Create("electronicvoting.validator_b")
        };

        foreach (var validatorInstance in validatorInstances)
            await validatorInstance.InitializeAsync();
        
        var costVoteId = Guid.NewGuid();
        var command = VoteValidatedTestHelper.CreateCastVoteCommand(costVoteId);
        var responseMessage = await VoteValidatedTestHelper.SendCastVoteAsync(validatorInstances[0].ApiHostPort, command);
        Assert.Equal(System.Net.HttpStatusCode.OK, responseMessage.StatusCode);
        
        var connectionStringValidatorA = validatorInstances[0].ValidatorDbContainer.GetConnectionString();
        var dbContextValidatorA = DbContextFactory.Create<ValidatorLedgerDbContext>(connectionStringValidatorA);
        RepositoriesFactory repositoriesFactoryApiA = new (dbContextValidatorA);
        
        var connectionStringValidatorB = validatorInstances[1].ValidatorDbContainer.GetConnectionString();
        var dbContextValidatorB = DbContextFactory.Create<ValidatorLedgerDbContext>(connectionStringValidatorB);
        RepositoriesFactory repositoriesFactoryApiB = new (dbContextValidatorB);
        
        await Task.Delay(TimeSpan.FromSeconds(4));
        
        var voteEncryptionForValidatorA = await repositoriesFactoryApiA.VoteEncryptionRepository.GetByIdAsync(costVoteId);
        var voteEncryptionForValidatorB = await repositoriesFactoryApiB.VoteEncryptionRepository.GetByIdAsync(costVoteId);
        Assert.NotNull(voteEncryptionForValidatorA);
        Assert.NotNull(voteEncryptionForValidatorB);

        var voteValidationProcessApiA = await repositoriesFactoryApiA.VoteValidationProcessRepository.GetByVoteEncryptionIdAsync(costVoteId);
        var voteValidationProcessApiB = await repositoriesFactoryApiB.VoteValidationProcessRepository.GetByVoteEncryptionIdAsync(costVoteId);
        Assert.NotNull(voteValidationProcessApiA);
        Assert.NotNull(voteValidationProcessApiB);
        
        var localVoteValidationProcessA = await repositoriesFactoryApiA.LocalVoteValidationProcessRepository.GetByVoteValidationProcessIdAsync(voteValidationProcessApiA.Id);
        var localVoteValidationProcessB = await repositoriesFactoryApiB.LocalVoteValidationProcessRepository.GetByVoteValidationProcessIdAsync(voteValidationProcessApiB.Id);
        Assert.NotNull(localVoteValidationProcessA);
        Assert.NotNull(localVoteValidationProcessB);
    }
}