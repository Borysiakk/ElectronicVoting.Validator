namespace ElectronicVoting.Validator.Domain.Models;

public record VoteEncryptionDetails
{
    public long R { get; set; }
    public string Vote { get; set; }
}