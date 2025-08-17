using ElectronicVoting.Validator.Domain.Models.Election;
namespace ElectronicVoting.Validator.Application.DTO.VoteValidation;

public class VoteEncryptionDto
{
    public Guid Id { get; set; }
    public VoteEncryption VoteEncryption { get; set; }
}