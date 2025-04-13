
using ElectronicVoting.Validator.Application.Handlers.Commands.Blockchain;
using ElectronicVoting.Validator.Application.RestApi;
using ElectronicVoting.Validator.Domain.Models.Blockchain;
using ElectronicVoting.Validator.Infrastructure.Refit;
using ElectronicVoting.Validator.Infrastructure.Repository;

namespace ElectronicVoting.Validator.Application.Service;


public interface IElectionConsensusService
{
     Task ExecutePrePrepareAsync(PendingBlockDto pendingBlockDto, CancellationToken cancellationToken);
     Task ExecutePrepareAsync(PrepareBlock prepareBlock, CancellationToken cancellationToken);
     // Task ExecuteCommitAsync(CommitBlock commitBlock, CancellationToken cancellationToken);
}

public class ElectionConsensusService: IElectionConsensusService
{
    private readonly INodeRepository _nodeRepository;
    private readonly HttpApiClientFactory _httpApiClientFactory;
    
    public ElectionConsensusService(HttpApiClientFactory httpApiClientFactory, INodeRepository nodeRepository)
    {
        _nodeRepository = nodeRepository;
        _httpApiClientFactory = httpApiClientFactory;
    }

    public async Task ExecutePrePrepareAsync(PendingBlockDto pendingBlockDto, CancellationToken cancellationToken)
    {
        var nodes = await _nodeRepository.FindOtherHostsAsync(cancellationToken);

        var executionTasks = nodes.Select(async node =>
        {
            var consensusApi = _httpApiClientFactory.CreateClient<IElectionConsensusApi>(node.Host);
            return await consensusApi.PrePrepareBlockAsync(pendingBlockDto);
        });
        await Task.WhenAll(executionTasks);
    }

    public async Task ExecutePrepareAsync(PrepareBlock prepareBlock, CancellationToken cancellationToken)
    {
        var nodes = await _nodeRepository.FindOtherHostsAsync(cancellationToken);
        var executionTasks = nodes.Select(async node =>
        {
            var consensusApi = _httpApiClientFactory.CreateClient<IElectionConsensusApi>(node.Host);
            return await consensusApi.PrepareBlockAsync(prepareBlock);
        });
        
        var t = await Task.WhenAll(executionTasks);
    }
    //
    // public async Task ExecuteCommitAsync(CommitBlock commitBlock, CancellationToken cancellationToken)
    // {
    //     var nodes = await _nodeRepository.FindOtherHostsAsync(cancellationToken);
    //     var executionTasks = nodes.Select(async node =>
    //     {
    //         var consensusApi = _httpApiClientFactory.CreateClient<IElectionConsensusApi>(node.Host);
    //         return await consensusApi.CommitBlock(commitBlock);
    //     });
    //     
    //     var t = await Task.WhenAll(executionTasks);
    // }
}