
using ElectronicVoting.Validator.Application.Handlers.Commands.Blockchain;
using ElectronicVoting.Validator.Domain.Models.Blockchain;
using Refit;

namespace ElectronicVoting.Validator.Application.RestApi;

public interface IElectionConsensusApi
{
    [Post("/api/ElectionConsensus/pre-prepare-block")]
    public Task<HttpResponseMessage> PrePrepareBlockAsync([Body] PendingBlockDto pendingBlockDto);
    
    [Post("/api/ElectionConsensus/prepare-block")]
    public Task<HttpResponseMessage> PrepareBlockAsync([Body] PrepareBlock prepareBlock);
    //
    // [Post("/api/ElectionConsensus/commit-block")]
    // public Task<HttpResponseMessage> CommitBlock([Body] CommitBlock commitBlock);
}