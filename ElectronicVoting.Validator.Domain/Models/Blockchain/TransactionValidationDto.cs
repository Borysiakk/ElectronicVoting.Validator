using System.Text.Json.Serialization;

namespace ElectronicVoting.Validator.Domain.Models.Blockchain;

public class TransactionValidationDto
{
    public string Data { get; set; }
    public long TransactionId { get; set; }
}