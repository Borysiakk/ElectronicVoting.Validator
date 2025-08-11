using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using ElectronicVoting.Validator.Domain.Models.BlockValidation;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.BlockValidation;

public class StartLocalBlockValidationCommand
{
    public PendingBlock PendingBlock { get; set; }
}

public class StartLocalBlockValidationHandler
{
    
}