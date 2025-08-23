namespace ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;

public class BlockValidationResultEntity: Entity<Guid>
{
    public bool IsValid { get; set; }
    public string Hash { get; set; }
    public Guid ValidatorId { get; set; }
    public bool IsLeaderValidation { get; set; }
    public IReadOnlyList<string> RejectionReason { get; set; }

    public Guid PendingBlockEntityId { get; set; }
    public virtual PendingBlockEntity PendingBlockEntity { get; set; }
}