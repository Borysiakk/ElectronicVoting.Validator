namespace ElectronicVoting.Validator.Domain.Models.Blockchain;

public class PendingBlockDto
{
    public long Id { get; set; }
    public string Hash { get; set; }
    public string PreviousHash { get; set; }
    public long PbftSequenceNumber { get; set; }
    public List<PendingTransactionDto> PendingTransaction { get; set; }
}