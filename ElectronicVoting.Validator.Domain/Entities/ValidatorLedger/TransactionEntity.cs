namespace ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;

public class TransactionEntity: Entity<Guid>
{
    public long BlockId { get; set; }
    public BlockEntity Block { get; set; }
}