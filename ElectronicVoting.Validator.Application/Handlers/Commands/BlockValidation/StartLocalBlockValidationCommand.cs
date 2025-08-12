using ElectronicVoting.Validator.Domain.Commands;
namespace ElectronicVoting.Validator.Application.Handlers.Commands.BlockValidation;

public class StartLocalBlockValidationHandler
{
    public async Task HandleAsync(StartLocalBlockValidationCommand command, CancellationToken ct)
    {
        Console.WriteLine("Start Local Block Validation");
    }
}