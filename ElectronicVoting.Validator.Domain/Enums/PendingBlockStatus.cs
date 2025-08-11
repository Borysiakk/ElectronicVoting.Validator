namespace ElectronicVoting.Validator.Domain.Enums;

public enum  PendingBlockStatus
{
    Created,
    Processed,
    ReadyToCommit,
    Committed,
    Rejected,
    Cancelled
}