using ElectronicVoting.Validator.Application.Handlers.Commands.Election;
using ElectronicVoting.Validator.Application.RestApi;
using ElectronicVoting.Validator.Domain.Models.Election;
using Refit;

namespace ElectronicVoting.Validator.Test.IntegrationTests;

public static class VoteValidatedTestHelper
{
    public static async Task<HttpResponseMessage> SendCastVoteAsync(ushort port, CastVoteCommand command)
    {
        var api = RestService.For<IElectionApi>($"http://localhost:{port}");
        return await api.CastVoteAsync(command);
    }
    
    public static CastVoteCommand CreateCastVoteCommand(Guid? id = null)
    {
        return new CastVoteCommand
        {
            Id = id ?? Guid.NewGuid(),
            VoteEncryption = new VoteEncryption
            {
                VoteEncryptionDetails = new VoteEncryptionDetails
                {
                    R = 1,
                    Vote = "1"
                },
                VoteProofOfKnowledgeBase = new VoteProofOfKnowledgeBase
                {
                    E = new[] { "1" },
                    U = new[] { "1" },
                    V = new[] { "1" }
                }
            }
        };
    }
}