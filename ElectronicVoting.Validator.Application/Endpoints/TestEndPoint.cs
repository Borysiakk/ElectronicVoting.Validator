using Wolverine.Http;

namespace ElectronicVoting.Validator.Application.Endpoints;

public static class TestEndPoint
{
    [WolverineGet("api/test")]
    public static string CastVote()
    {
        return "OK!";
    }
}