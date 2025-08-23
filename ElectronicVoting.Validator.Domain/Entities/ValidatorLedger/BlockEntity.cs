using ElectronicVoting.Validator.Domain.Interface;

namespace ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;

public class BlockEntity: Entity<long>
{
    public string Hash { get; set; }
    public string PreviousHash { get; set; }
    public long PbftSequenceNumberId { get; set; }
    public virtual PbftSequenceEntity PbftSequence { get; set; }
    public ICollection<TransactionEntity> Transactions { get; set; }
}