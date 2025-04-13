namespace ElectronicVoting.Validator.Domain.Entities.Blockchain;

public class Block: Entity
{
    public string Hash { get; set; }
    public string PreviousHash { get; set; }
    public long PbftSequenceNumber { get; set; }
    public ICollection<Transaction> Transactions { get; set; }
}