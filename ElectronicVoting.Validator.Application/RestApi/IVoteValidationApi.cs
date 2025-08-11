using ElectronicVoting.Validator.Application.Handlers.Commands.VoteValidation;
using Refit;

namespace ElectronicVoting.Validator.Application.RestApi;

public interface IVoteValidationApi
{
    [Post("/api/vote-validation/local/start")]
    public Task<HttpResponseMessage> StartLocalVoteValidationAsync([Body] StartLocalVoteValidationCommand command);
    
    [Post("/api/vote-validation/local/receive")]
    public Task<HttpResponseMessage> ReceiveLocalVoteValidationAsync([Body] ReceiveLocalVoteValidationCommand command);
    
    [Post("/api/vote-validation/leader/receive-consensus")]
    public Task<HttpResponseMessage> LeaderReceiveVoteConsensusReportAsync([Body] LeaderReceiveVoteConsensusReportCommand command);
    
    [Post("/api/vote-validation/local/finalize")]
    public Task<HttpResponseMessage> FinalizeLocalVoteValidationAsync([Body] FinalizeLocalVoteValidationCommand command);
}
