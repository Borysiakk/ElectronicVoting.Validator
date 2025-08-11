using ElectronicVoting.Validator.Infrastructure.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace ElectronicVoting.Validator.Infrastructure.Services;

public static class Extensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection service)
    {
        service.AddScoped<IPendingBlockFactory, PendingBlockFactory>();
        service.AddScoped<IPbftBlockCreatorProcess, PbftBlockCreatorProcess>();
        service.AddScoped<IVoteValidationTimeoutProcessor, VoteValidationTimeoutProcessor>();
        return service;
    }
}