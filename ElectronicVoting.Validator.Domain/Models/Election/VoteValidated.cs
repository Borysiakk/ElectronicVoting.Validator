using ElectronicVoting.Validator.Domain.Interface;

namespace ElectronicVoting.Validator.Domain.Models.Election;

public class VoteValidated: ISignable
{
    public bool IsValid { get; set; }
    public string Message { get; set; }
    public Guid VoteValidationId { get; set; }
    public Guid VoteEncryptionId { get; set; }
}