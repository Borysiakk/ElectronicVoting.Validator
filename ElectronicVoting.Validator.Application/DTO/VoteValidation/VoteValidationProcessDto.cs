using ElectronicVoting.Validator.Domain.Enums;

namespace ElectronicVoting.Validator.Application.DTO.VoteValidation;

public class VoteValidationProcessDto
{
    public Guid Id { get; set; }
    public Guid VoteEncryptionId { get; set; }
    public VoteValidationProcessStatus Status { get; set; }
    public DateTime StartedAt { get; set; } 
    public DateTime FinishedAt { get; set; }
}