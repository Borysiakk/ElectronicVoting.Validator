using ElectronicVoting.Validator.Application.Handlers.Commands.Election;
using Microsoft.AspNetCore.Http;
using Wolverine;
using Wolverine.Http;

namespace ElectronicVoting.Validator.Application.Endpoints;

public static class CastVoteEndpoint
{
    [WolverinePost("api/election/cast-vote")]
    public static async Task<IResult> CastVote(CastVoteCommand command, IMessageBus bus)
    {
        await bus.SendAsync(command);
        return Results.Ok();
    }
}