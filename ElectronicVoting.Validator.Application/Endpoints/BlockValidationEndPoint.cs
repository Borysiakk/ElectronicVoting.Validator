using ElectronicVoting.Validator.Application.Handlers.Commands.BlockValidation;
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

    [WolverinePost("api/block-validation/local/receive")]
    public static async Task ReceiveLocalBlockValidationEndPoint(ReceiveLocalBlockValidationCommand command, IMessageBus bus)
    {
        Console.WriteLine("ReceiveLocalBlockValidationEndPoint");
        await bus.SendAsync(command);
    }
    
    public static async Task LeaderReceiveBlockConsensusReportEndPoint(LeaderReceiveBlockConsensusReportCommand command, IMessageBus bus)
    {
        Console.WriteLine("LeaderReceiveBlockConsensusReportEndPoint");
        await bus.SendAsync(command);
    }
}
