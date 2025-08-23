using ElectronicVoting.Validator.Application.Factories;
using ElectronicVoting.Validator.Application.Processes;
using ElectronicVoting.Validator.Application.Services;
using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using ElectronicVoting.Validator.Domain.Enums;
using ElectronicVoting.Validator.Domain.Models.Election;
using ElectronicVoting.Validator.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Wolverine;
using Xunit;

namespace ElectronicVoting.Validator.Test.UnitTests.Processes;

public class PbftBlockCreatorProcessTest: ValidatorLedgerDbTestBase
{
    [Fact]
    public async Task Test1()
    {
        var encryptionEntity = CreateVoteEncryptionAsync();
        await RepositoriesFactory.VoteEncryptionRepository.AddAsync(encryptionEntity, CancellationToken.None);
        
        var voteValidationProcess = CreateVoteValidationProcessAsync(encryptionEntity.Id);
        await RepositoriesFactory.VoteValidationProcessRepository.AddAsync(voteValidationProcess, CancellationToken.None);
        
        await DbContext.SaveChangesAsync(CancellationToken.None);
        Mock<IMessageBus> mockMessageBus = new();
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
        
        PbftBlockCreatorProcess blockCreatorProcess = new (mockMessageBus.Object, pendingBlockFactory, RepositoriesFactory.VoteValidationProcessRepository);
        
        await blockCreatorProcess.ProcessAsync(CancellationToken.None);
    }
    
    private VoteValidationProcessEntity CreateVoteValidationProcessAsync(Guid voteEncryptionId)
    {
        return new VoteValidationProcessEntity 
        {
            Id = Guid.NewGuid(),
            VoteEncryptionId = voteEncryptionId,
            Status = VoteValidationProcessStatus.ReadyToCommit,
            StartedAt = DateTime.UtcNow.AddMinutes(-5),
            FinishedAt = DateTime.UtcNow.AddMinutes(10),
        };
    }
    
    private VoteEncryptionEntity CreateVoteEncryptionAsync()
    {
        return new VoteEncryptionEntity()
        {
            Id = Guid.NewGuid(),
            VoteEncryption = new VoteEncryption()
            {
                VoteEncryptionDetails = new VoteEncryptionDetails()
                {
                    R = 1,
                    Vote = "1"
                },
                VoteProofOfKnowledgeBase = new VoteProofOfKnowledgeBase()
                {
                    E = new[] { "1" },
                    U = new[] { "1" },
                    V = new[] { "1" }
                }
            }
        };
    }
}