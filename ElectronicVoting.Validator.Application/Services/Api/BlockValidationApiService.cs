using ElectronicVoting.Validator.Application.RestApi;
using ElectronicVoting.Validator.Domain.Commands;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.Election.Repositories;
using ElectronicVoting.Validator.Infrastructure.Rafit;

namespace ElectronicVoting.Validator.Application.Services.Api;

public interface IBlockValidationApiService
{
    Task BroadcastStartLocalVoteValidationAsync(StartLocalBlockValidationCommand command, CancellationToken cancellationToken);
}

public class BlockValidationApiService(
    IHttpApiClientFactory httpApiClientFactory,
    IElectionValidatorService electionValidatorService,
    IValidatorNodeRepository validatorNodeRepository)
    : IBlockValidationApiService
{
    
    public async Task BroadcastStartLocalVoteValidationAsync(StartLocalBlockValidationCommand command, CancellationToken cancellationToken)
    {
        var validators = await validatorNodeRepository.GetAllAsync(cancellationToken);
        var executionTasks = validators.Select(async validator =>
        {
            var api = httpApiClientFactory.CreateClient<IBlockValidationApi>(validator.ServerUrl);
            return await api.StartLocalBlockValidationAsync(command);
        });
        await Task.WhenAll(executionTasks);
    }
}