using ElectronicVoting.Validator.Application.DTO.BlockValidation;
using ElectronicVoting.Validator.Domain.Interface;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.BlockValidation;

public class StartLocalBlockValidationCommand :ISignedCommand
{
    public string Signature { get; set; }
    public Guid SignedByValidatorId { get; set; }
    
    public PendingBlockDto PendingBlock { get; set; }
}

public class StartLocalBlockValidationHandler
{
    public async Task HandleAsync(StartLocalBlockValidationCommand command, bool isValidSigned, CancellationToken ct)
    {
        Console.WriteLine("Start Local Block Validation");
    }
}