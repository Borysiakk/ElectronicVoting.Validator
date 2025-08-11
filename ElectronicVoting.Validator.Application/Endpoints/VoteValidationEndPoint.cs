using ElectronicVoting.Validator.Application.Handlers.Commands.VoteValidation;
using Wolverine;
using Wolverine.Http;

namespace ElectronicVoting.Validator.Application.Endpoints;

public static class VoteValidationEndPoint
{
    [WolverinePost("api/vote-validation/local/start")]
    public static async Task StartLocalVoteValidationEndPoint(StartLocalVoteValidationCommand command, IMessageBus bus)
    {
        Console.WriteLine("StartLocalVoteValidationEndPoint");
        await bus.SendAsync(command);
    }
    
    [WolverinePost("api/vote-validation/local/finalize")]
    public static async Task FinalizeLocalVoteValidationEndPoint(FinalizeLocalVoteValidationCommand command, IMessageBus bus)
    {
        Console.WriteLine("FinalizeLocalVoteValidationEndPoint");
        await bus.SendAsync(command);
    }

    [WolverinePost("api/vote-validation/local/receive")]
    public static async Task ReceiveLocalVoteValidationEndPoint(ReceiveLocalVoteValidationCommand command, IMessageBus bus)
    {
        Console.WriteLine("ReceiveLocalVoteValidationEndPoint");
        await bus.SendAsync(command);
    }
    
    [WolverinePost("api/vote-validation/leader/receive-consensus")]
    public static async Task LeaderReceiveVoteConsensusReportEndPoint(LeaderReceiveVoteConsensusReportCommand command, IMessageBus bus)
    {
        Console.WriteLine("LeaderReceiveVoteConsensusReportEndPoint");
        await bus.SendAsync(command);
    }
}