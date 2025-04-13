using System.Text.Json.Serialization;

namespace ElectronicVoting.Validator.Domain.Models.Blockchain;

public class BlockValidationDto
{
    public long BlockId { get; set; }
    public string Hash { get; set; }
    public string PreviousHash { get; set; }
    public long PbftSequenceNumber { get; set; }
    public List<PendingTransactionDto> Transactions { get; set; }
}