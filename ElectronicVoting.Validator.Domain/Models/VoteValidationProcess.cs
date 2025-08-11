using ElectronicVoting.Validator.Domain.Enums;

namespace ElectronicVoting.Validator.Domain.Models;

public class VoteValidationProcess
{
    public Guid VoteEncryptionId { get; set; }
    public VoteValidationProcessStatus Status { get; set; }
    public DateTime StartedAt { get; set; } 
    public DateTime FinishedAt { get; set; }
}
