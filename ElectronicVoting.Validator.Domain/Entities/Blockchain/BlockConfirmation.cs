using ElectronicVoting.Validator.Domain.Enum;

namespace ElectronicVoting.Validator.Domain.Entities.Blockchain;

public class BlockConfirmation: Entity
{
    public long PendingBlockId { get; set; }
    public ConfirmationType Type { get; set; }

}