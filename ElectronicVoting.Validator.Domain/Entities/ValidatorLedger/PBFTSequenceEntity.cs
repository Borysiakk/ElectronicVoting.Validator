namespace ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;

public class PbftSequenceEntity: Entity<long>
{
    public long SequenceNumber { get; set; }
    public virtual BlockEntity Block { get; set; }
}