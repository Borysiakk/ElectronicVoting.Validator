using ElectronicVoting.Validator.Domain.Enums;
namespace ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;

public class PendingBlockEntity: Entity<Guid>
{
    public string Hash { get; set; }
    public DateTime StartedAt { get; set; } 
    public DateTime ?FinishedAt { get; set; }
    
    public PendingBlockStatus Status { get; set; }
}