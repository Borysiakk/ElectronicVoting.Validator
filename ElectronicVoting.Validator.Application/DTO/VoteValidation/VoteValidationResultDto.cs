namespace ElectronicVoting.Validator.Application.DTO.VoteValidation;

public class VoteValidationResultDto
{
    public bool IsValid { get; set; }
    public Guid VoteValidationProcessId { get; set; }
    public Guid VoteEncryptionId { get; set; }
    
    public DateTime StartedAt { get; set; } 
    public DateTime FinishedAt { get; set; }
    
}