using ElectronicVoting.Validator.Domain.Commands;
using Wolverine;
using Wolverine.Http;

namespace ElectronicVoting.Validator.Application.Endpoints;

public class BlockValidationEndPoint
{
    [WolverinePost("api/block-validation/local/start")]
    public static async Task StartLocalBlockValidationEndPoint(StartLocalBlockValidationCommand command, IMessageBus bus)
    {
        Console.WriteLine("StartLocalBlockValidationEndPoint");
        await bus.SendAsync(command);
    }
}