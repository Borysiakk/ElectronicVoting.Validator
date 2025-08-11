using ElectronicVoting.Validator.Domain.Enums;

namespace ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;

public class VoteValidationProcessEntity: Entity<Guid>
{
    public Guid VoteEncryptionId { get; set; }
    public VoteValidationProcessStatus Status { get; set; }
    public DateTime StartedAt { get; set; } 
    public DateTime FinishedAt { get; set; }
}