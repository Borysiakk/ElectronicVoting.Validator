using ElectronicVoting.Validator.Application.Handlers.Commands.BlockValidation;
using ElectronicVoting.Validator.Application.RestApi;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.Election.Repositories;
using ElectronicVoting.Validator.Infrastructure.Rafit;

namespace ElectronicVoting.Validator.Application.Services.Api;

public interface IBlockValidationApiService
{
    Task BroadcastStartLocalVoteValidationAsync(StartLocalBlockValidationCommand command, CancellationToken cancellationToken);
    Task BroadcastReceiveLocalBlockValidationAsync(ReceiveLocalBlockValidationCommand command, CancellationToken cancellationToken);
    Task BroadcastLeaderReceiveBlockConsensusReportAsync(LeaderReceiveBlockConsensusReportCommand command, CancellationToken ct);
}

public class BlockValidationApiService(
    IHttpApiClientFactory httpApiClientFactory,
    IValidatorNodeRepository validatorNodeRepository,
    IElectionValidatorService electionValidatorService)
    : IBlockValidationApiService
{
    
    public async Task BroadcastStartLocalVoteValidationAsync(StartLocalBlockValidationCommand command, CancellationToken cancellationToken)
    {
        var validators = await electionValidatorService.GetAllValidatorsExceptCurrent(cancellationToken);
        var executionTasks = validators.Select(async validator =>
        {
            var api = httpApiClientFactory.CreateClient<IBlockValidationApi>(validator.ServerUrl);
            return await api.StartLocalBlockValidationAsync(command);
        });
        await Task.WhenAll(executionTasks);
    }
    
    public async Task BroadcastReceiveLocalBlockValidationAsync(ReceiveLocalBlockValidationCommand command, CancellationToken cancellationToken)
    {
        var validators = await electionValidatorService.GetAllValidatorsExceptCurrent(cancellationToken);
        var executionTasks = validators.Select(async validator =>
        {
            var api = httpApiClientFactory.CreateClient<IBlockValidationApi>(validator.ServerUrl);
            return await api.ReceiveLocalBlockValidationAsync(command);
        });
        
        await Task.WhenAll(executionTasks);
    }

    public async Task BroadcastLeaderReceiveBlockConsensusReportAsync(LeaderReceiveBlockConsensusReportCommand command, CancellationToken ct)
    {
        var leader = await validatorNodeRepository.GetLeaderAsync(ct);
        if (leader == null)
            throw new NullReferenceException("Nie udalo pobrac sie lidera");
        
        var api = httpApiClientFactory.CreateClient<IBlockValidationApi>(leader.ServerUrl);
        await api.LeaderReceiveVoteConsensusReportAsync(command);
    }
}