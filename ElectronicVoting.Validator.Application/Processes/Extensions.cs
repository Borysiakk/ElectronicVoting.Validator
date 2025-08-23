using ElectronicVoting.Validator.Domain.Interface.Processes;

using Microsoft.Extensions.DependencyInjection;

namespace ElectronicVoting.Validator.Application.Processes;

public static class Extensions
{
    public static IServiceCollection AddApplicationProcesses(this IServiceCollection service)
    {
        service.AddScoped<IPbftBlockCreatorProcess, PbftBlockCreatorProcess>();
        service.AddScoped<IPbftBlockConsensusProcessor, PbftBlockConsensusProcessor>();
        service.AddScoped<IPbftBlockValidationProcess, PbftBlockValidationProcess>();
        service.AddScoped<IVoteValidationTimeoutProcessor, VoteValidationTimeoutProcessor>();
        return service;
    }
}