using ElectronicVoting.Validator.Application.DTO.VoteValidation;

namespace ElectronicVoting.Validator.Application.DTO.BlockValidation;

public class PendingTransactionDetailsDto
{
    public Guid Id { get; set; }
    public VoteEncryptionDto VoteEncryption { get; set; }
    public VoteValidationProcessDto VoteValidationProcess { get; set; }
}