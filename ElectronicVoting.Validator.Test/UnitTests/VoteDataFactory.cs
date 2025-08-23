using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using ElectronicVoting.Validator.Domain.Enums;
using ElectronicVoting.Validator.Domain.Models.Election;

namespace ElectronicVoting.Validator.Test.UnitTests;

public static class VoteDataFactory
{
    public static VoteEncryptionEntity CreateVoteEncryption()
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

    public static VoteValidationProcessEntity CreateVoteValidationProcess(Guid voteEncryptionId, VoteValidationProcessStatus status)
    {
        return new VoteValidationProcessEntity()
        {
            Id = Guid.NewGuid(),
            Status = status,
            VoteEncryptionId = voteEncryptionId,
            StartedAt = DateTime.UtcNow,
            FinishedAt = DateTime.UtcNow.AddMinutes(10)
        };
    }

}