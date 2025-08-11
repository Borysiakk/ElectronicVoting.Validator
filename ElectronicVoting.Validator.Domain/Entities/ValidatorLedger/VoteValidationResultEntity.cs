namespace ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;

public class VoteValidationResultEntity: Entity<Guid>
{
    public bool IsValid { get; set; }
    public Guid ValidatorId { get; set; }
    public Guid VoteEncryptionId { get; set; }
    public Guid VoteValidationProcessId { get; set; }
    public Guid LocalVoteValidationProcessId { get; set; }
}