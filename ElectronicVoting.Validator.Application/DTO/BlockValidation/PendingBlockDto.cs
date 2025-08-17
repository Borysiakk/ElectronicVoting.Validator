namespace ElectronicVoting.Validator.Application.DTO.BlockValidation;

public class PendingBlockDto
{
    public Guid Id { get; set; }
    public string Hash { get; set; }
    public string PreviousHash { get; set; }
    
    public PbftSequenceDto PbftSequence { get; set; }
    public List<PendingTransactionDto> PendingTransactions { get; set; }
}