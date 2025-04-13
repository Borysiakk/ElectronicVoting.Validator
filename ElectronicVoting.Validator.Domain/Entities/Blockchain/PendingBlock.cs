namespace ElectronicVoting.Validator.Domain.Entities.Blockchain;

public class PendingBlock: Entity
{
    public string? Hash { get; set; }
    public string PreviousHash { get; set; }
    public long PbftSequenceNumber { get; set; }
    public bool IsProcessed { get; set; }
    
    public ICollection<BlockConfirmation> BlockConfirmations { get; set; }
    public ICollection<PendingTransaction> PendingTransactions { get; set; }
}