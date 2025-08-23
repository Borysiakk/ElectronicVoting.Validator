using ElectronicVoting.Validator.Application.Handlers.Commands.BlockValidation;
using Refit;

namespace ElectronicVoting.Validator.Application.RestApi;

public interface IBlockValidationApi
{
    [Post("/api/block-validation/local/start")]
    public Task<HttpResponseMessage> StartLocalBlockValidationAsync([Body] StartLocalBlockValidationCommand command);
    
    [Post("/api/block-validation/local/receive")]
    public Task<HttpResponseMessage> ReceiveLocalBlockValidationAsync([Body] ReceiveLocalBlockValidationCommand command);
    
    [Post("/api/block-validation/leader/receive-consensus")]
    public Task<HttpResponseMessage> LeaderReceiveVoteConsensusReportAsync([Body] LeaderReceiveBlockConsensusReportCommand command);
}
