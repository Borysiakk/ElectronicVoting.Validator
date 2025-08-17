using ElectronicVoting.Validator.Domain.Enums;
using ElectronicVoting.Validator.Domain.Interface; 
namespace ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;

public class PendingBlockEntity: Entity<Guid>, IHashable
{
    public string Hash { get; set; }
    public string PreviousHash { get; set; }
    
    public PendingBlockStatus Status { get; set; }
    public DateTime StartedAt { get; set; } 
    public DateTime? FinishedAt { get; set; }
    
    public long PbftSequenceNumberId { get; set; }
    public virtual PbftSequenceEntity PbftSequence { get; set; }
    public virtual ICollection<PendingTransactionEntity> PendingTransactions { get; set; }
    
    public object GetHashSource()
    {
        return new
        {
            StartedAt,
            PreviousHash,
            PendingTransactions,
        };
    }
}