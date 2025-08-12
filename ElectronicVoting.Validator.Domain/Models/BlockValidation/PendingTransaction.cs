namespace ElectronicVoting.Validator.Domain.Models.BlockValidation;

public class PendingTransaction
{
    public Guid VoteEncryptionId { get; set; }
    public Guid VoteValidationProcessId { get; set; }
}