namespace ElectronicVoting.Validator.Domain.Enums;

public enum LocalVoteValidationStatus
{
    Registered,
    InProgress,
    Completed,
    Rejected,
    Timeout,
    Cancelled
}