namespace ElectronicVoting.Validator.Application.DTO.BlockValidation;

public class PendingTransactionDto
{
    public Guid Id { get; set; }
    public Guid VoteEncryptionId { get; set; }
    public Guid VoteValidationProcessId { get; set; }
}