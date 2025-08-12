using ElectronicVoting.Validator.Domain.Models.BlockValidation;

namespace ElectronicVoting.Validator.Domain.Commands;

public record LeaderInitiateBlockValidationCommand
{
    public PendingBlock PendingBlock { get; set; }
}