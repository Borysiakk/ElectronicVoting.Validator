using ElectronicVoting.Validator.Application.Handlers.Commands.Election;
using Refit;

namespace ElectronicVoting.Validator.Application.RestApi;

public interface IElectionApi
{
    [Post("/api/election/cast-vote")]
    public Task<HttpResponseMessage> CastVoteAsync([Body] CastVoteCommand command);
}