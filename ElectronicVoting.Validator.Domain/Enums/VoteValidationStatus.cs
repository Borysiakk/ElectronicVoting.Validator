namespace ElectronicVoting.Validator.Domain.Enums;

public enum VoteValidationProcessStatus
{
    Registered,
    InProgress,
    ReadyToCommit,
    ProcessedToCommit,
    Committed,
    Completed,
    Rejected,
    Timeout,
    Cancelled
}