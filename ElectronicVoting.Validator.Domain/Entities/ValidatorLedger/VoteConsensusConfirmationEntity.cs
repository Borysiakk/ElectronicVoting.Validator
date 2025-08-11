namespace ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;

public class VoteConsensusConfirmationEntity: Entity<Guid>
{
    public Guid ValidatorId { get; set; }
    public Guid VoteEncryptionId { get; set; }
    public Guid VoteValidationProcessId { get; set; }
}