using ElectronicVoting.Validator.Application.Handlers.Commands.VoteValidation;
using ElectronicVoting.Validator.Application.RestApi;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.Election.Repositories;
using ElectronicVoting.Validator.Infrastructure.Rafit;

namespace ElectronicVoting.Validator.Application.Services.Api;

public interface IValidationVoteApiService
{
    Task BroadcastStartLocalVoteValidationAsync(StartLocalVoteValidationCommand command, CancellationToken cancellationToken);
    Task BroadcastFinalizeLocalVoteValidationAsync(FinalizeLocalVoteValidationCommand command, CancellationToken cancellationToken);
    Task BroadcastReceiveLocalVoteValidationAsync(ReceiveLocalVoteValidationCommand command, CancellationToken cancellationToken);
    Task BroadcastLeaderReceiveVoteConsensusReportAsync(LeaderReceiveVoteConsensusReportCommand command, CancellationToken ct);
}
public class ValidationVoteApiService: IValidationVoteApiService
{
    private readonly IHttpApiClientFactory _httpApiClientFactory;
    private readonly IElectionValidatorService _electionValidatorService;
    private readonly IValidatorNodeRepository _validatorNodeRepository;

    public ValidationVoteApiService(IHttpApiClientFactory httpApiClientFactory, IElectionValidatorService electionValidatorService, IValidatorNodeRepository validatorNodeRepository)
    {
        _httpApiClientFactory = httpApiClientFactory;
        _electionValidatorService = electionValidatorService;
        _validatorNodeRepository = validatorNodeRepository;
    }

    public async Task BroadcastStartLocalVoteValidationAsync(StartLocalVoteValidationCommand command, CancellationToken cancellationToken)
    {
        var validators = await _validatorNodeRepository.GetAllAsync(cancellationToken);
        var executionTasks = validators.Select(async validator =>
        {
            var api = _httpApiClientFactory.CreateClient<IVoteValidationApi>(validator.ServerUrl);
            return await api.StartLocalVoteValidationAsync(command);
        });
        await Task.WhenAll(executionTasks);
    }

    public async Task BroadcastFinalizeLocalVoteValidationAsync(FinalizeLocalVoteValidationCommand command, CancellationToken cancellationToken)
    {
        var allValidatorsExceptCurrent = await _electionValidatorService.GetAllValidatorsExceptCurrent(cancellationToken);
        var executionTasks = allValidatorsExceptCurrent.Select(async validator =>
        {
            var api = _httpApiClientFactory.CreateClient<IVoteValidationApi>(validator.ServerUrl);
            return await api.FinalizeLocalVoteValidationAsync(command);
        });
        await Task.WhenAll(executionTasks);
    }

    public async Task BroadcastReceiveLocalVoteValidationAsync(ReceiveLocalVoteValidationCommand command, CancellationToken cancellationToken)
    {
        var allValidatorsExceptCurrent = await _electionValidatorService.GetAllValidatorsExceptCurrent(cancellationToken);
        var executionTasks = allValidatorsExceptCurrent.Select(async validator =>
        {
            var api = _httpApiClientFactory.CreateClient<IVoteValidationApi>(validator.ServerUrl);
            return await api.ReceiveLocalVoteValidationAsync(command);
        });
        await Task.WhenAll(executionTasks);
    }

    public async Task BroadcastLeaderReceiveVoteConsensusReportAsync(LeaderReceiveVoteConsensusReportCommand command, CancellationToken ct)
    {
        var leader = await _validatorNodeRepository.GetLeaderAsync(ct);
        if (leader == null)
            throw new NullReferenceException("Nie udalo pobrac sie lidera");
        
        var api = _httpApiClientFactory.CreateClient<IVoteValidationApi>(leader.ServerUrl);
        await api.LeaderReceiveVoteConsensusReportAsync(command);
    }
}