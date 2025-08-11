using ElectronicVoting.Validator.Domain.Enums;

namespace ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;

public class LocalVoteValidationProcessEntity: Entity<Guid>
{
    public Guid VoteEncryptionId { get; set; }
    public Guid VoteValidationProcessId { get; set; }
    public LocalVoteValidationStatus Status { get; set; }
}