namespace ElectronicVoting.Validator.Domain.Models.BlockValidation;

public class PendingBlock
{
    public Guid Id { get; set; }
    public string Hash { get; set; }
    public string PreviousHash { get; set; }
    
    public long PbftSequenceNumberId { get; set; }
    
}