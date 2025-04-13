namespace ElectronicVoting.Validator.Domain.Entities.Blockchain;

public class Transaction: Entity
{
    public long BlockId { get; set; }
    public Block Block { get; set; }
}