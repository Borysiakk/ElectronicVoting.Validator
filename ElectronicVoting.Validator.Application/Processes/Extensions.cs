using ElectronicVoting.Validator.Domain.Interface.Processes;

using Microsoft.Extensions.DependencyInjection;

namespace ElectronicVoting.Validator.Application.Processes;

public static class Extensions
{
    public static IServiceCollection AddApplicationProcesses(this IServiceCollection service)
    {
        service.AddScoped<IPbftBlockCreatorProcess, PbftBlockCreatorProcess>();
        service.AddScoped<IVoteValidationTimeoutProcessor, VoteValidationTimeoutProcessor>();
        return service;
    }
}