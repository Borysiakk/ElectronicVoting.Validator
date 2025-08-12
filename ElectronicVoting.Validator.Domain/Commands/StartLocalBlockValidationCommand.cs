using ElectronicVoting.Validator.Domain.Interface;
using ElectronicVoting.Validator.Domain.Models.BlockValidation;

namespace ElectronicVoting.Validator.Domain.Commands;

public class StartLocalBlockValidationCommand :ISignedCommand
{
    public string Signature { get; set; }
    public Guid SignedByValidatorId { get; set; }
    
    public PendingBlock PendingBlock { get; set; }
}