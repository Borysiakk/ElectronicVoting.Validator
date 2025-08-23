using ElectronicVoting.Validator.Application.Processes;
using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using ElectronicVoting.Validator.Domain.Enums;
using ElectronicVoting.Validator.Domain.Models.Election;

using Xunit;

namespace ElectronicVoting.Validator.Test.UnitTests.Processes ;

public class VoteValidationTimeoutProcessorTest: ValidatorLedgerDbTestBase
{
    [Fact]
    public async Task ProcessAsync_WhenVoteValidationProcessIsFinished_ShouldSetStatusToTimeout()
    {
        var voteEncryption = VoteDataFactory.CreateVoteEncryption();
        await RepositoriesFactory.VoteEncryptionRepository.AddAsync(voteEncryption, CancellationToken.None);
        
        VoteValidationProcessEntity voteValidationProcess = new()
        {
            Id = Guid.NewGuid(),
            VoteEncryptionId = voteEncryption.Id,
            Status = VoteValidationProcessStatus.Registered,
            StartedAt = DateTime.UtcNow.AddMinutes(-5),
            FinishedAt = DateTime.UtcNow.AddMinutes(-1),
        };
        
        await RepositoriesFactory.VoteValidationProcessRepository.AddAsync(voteValidationProcess, CancellationToken.None);
        await DbContext.SaveChangesAsync(CancellationToken.None);

        VoteValidationTimeoutProcessor validationTimeoutProcessor = new(MockUnitOfWork.Object,
            RepositoriesFactory.VoteValidationProcessRepository,
            RepositoriesFactory.LocalVoteValidationProcessRepository);
        
        await validationTimeoutProcessor.ProcessAsync(CancellationToken.None);
        
        var updateVoteValidationProcess = await RepositoriesFactory.VoteValidationProcessRepository.GetByIdAsync(voteValidationProcess.Id, CancellationToken.None);
        Assert.Equal(VoteValidationProcessStatus.Timeout ,updateVoteValidationProcess.Status);
    }
    
    [Fact]
    public async Task ProcessAsync_WhenVoteValidationProcessIsNotFinished_ShouldKeepOriginalStatus()
    {
        var voteEncryption = VoteDataFactory.CreateVoteEncryption();
        await RepositoriesFactory.VoteEncryptionRepository.AddAsync(voteEncryption, CancellationToken.None);
        
        VoteValidationProcessEntity voteValidationProcess = new()
        {
            Id = Guid.NewGuid(),
            VoteEncryptionId = voteEncryption.Id,
            Status = VoteValidationProcessStatus.Registered,
            StartedAt = DateTime.UtcNow.AddMinutes(-5),
            FinishedAt = DateTime.UtcNow.AddMinutes(10),
        };
        
        await RepositoriesFactory.VoteValidationProcessRepository.AddAsync(voteValidationProcess, CancellationToken.None);
        await DbContext.SaveChangesAsync(CancellationToken.None);

        VoteValidationTimeoutProcessor validationTimeoutProcessor = new(MockUnitOfWork.Object,
            RepositoriesFactory.VoteValidationProcessRepository,
            RepositoriesFactory.LocalVoteValidationProcessRepository);
        
        await validationTimeoutProcessor.ProcessAsync(CancellationToken.None);
        
        var updateVoteValidationProcess = await RepositoriesFactory.VoteValidationProcessRepository.GetByIdAsync(voteValidationProcess.Id, CancellationToken.None);
        Assert.Equal(VoteValidationProcessStatus.Registered ,updateVoteValidationProcess.Status);
    }
}

