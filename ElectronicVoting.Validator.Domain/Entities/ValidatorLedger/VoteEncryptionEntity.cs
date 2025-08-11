using ElectronicVoting.Validator.Domain.Models.Election;

namespace ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;

public class VoteEncryptionEntity: Entity<Guid>
{
    public VoteEncryption VoteEncryption { get; set; }
}