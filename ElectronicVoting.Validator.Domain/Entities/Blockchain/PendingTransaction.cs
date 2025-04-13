namespace ElectronicVoting.Validator.Domain.Entities.Blockchain;

public class PendingTransaction :Entity
{
    public string Data { get; set; }
    public bool IsProcessed { get; set; }
    public long PendingBlockId { get; set; }
    public PendingBlock PendingBlock { get; set; }
}