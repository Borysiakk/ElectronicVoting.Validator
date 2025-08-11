namespace ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;

public class PendingTransactionEntity: Entity<Guid>
{
    public Guid PendingBlockId { get; set; }
    public virtual PendingBlockEntity PendingBlock { get; set; } 
    
    public Guid VoteEncryptionId { get; set; }
    public Guid VoteValidationProcessId { get; set; }
}