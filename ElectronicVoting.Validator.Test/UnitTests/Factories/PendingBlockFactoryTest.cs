using ElectronicVoting.Validator.Application.Factories;
using ElectronicVoting.Validator.Application.Services;
using ElectronicVoting.Validator.Domain.Enums;
using ElectronicVoting.Validator.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ElectronicVoting.Validator.Test.UnitTests.Factories;

public class PendingBlockFactoryTest: ValidatorLedgerDbTestBase
{
    [Fact]
    public async Task Test1()
    {
        var voteEncryptionEntity = VoteDataFactory.CreateVoteEncryption();
        var voteProcessEntity =
            VoteDataFactory.CreateVoteValidationProcess(voteEncryptionEntity.Id, VoteValidationProcessStatus.ReadyToCommit);
        
        
        await RepositoriesFactory.VoteEncryptionRepository.AddAsync(voteEncryptionEntity, CancellationToken.None);
        await RepositoriesFactory.VoteValidationProcessRepository.AddAsync(voteProcessEntity, CancellationToken.None);
        await DbContext.SaveChangesAsync(CancellationToken.None);
        
        Mock<IOptions<BlockValidationOptions>> mockBlockValidationOptions = new();
        mockBlockValidationOptions.Setup(x => x.Value).Returns(new BlockValidationOptions()
        {
            BlockValidationExpiryMinutes = 15,
        });
        Mock<IElectionValidatorService> mockElectionValidatorService = new();
        mockElectionValidatorService.Setup(x => x.GetCurrentValidatorIdAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Guid.NewGuid());
        
        PendingBlockFactory pendingBlockFactory = new(mockBlockValidationOptions.Object,
            RepositoriesFactory.BlockRepository, RepositoriesFactory.PbftSequenceRepository,
            mockElectionValidatorService.Object, RepositoriesFactory.VoteValidationProcessRepository);
        
        var validationProcessWhenReadyToCommit= await RepositoriesFactory.VoteValidationProcessRepository.GetByStatusAsync(VoteValidationProcessStatus.ReadyToCommit, CancellationToken.None);
        var result = await pendingBlockFactory.TryCreatePendingBlockAsync(validationProcessWhenReadyToCommit, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotNull(result.Value.Hash);
        Assert.NotNull(result.Value.PendingTransactions);
    }
}