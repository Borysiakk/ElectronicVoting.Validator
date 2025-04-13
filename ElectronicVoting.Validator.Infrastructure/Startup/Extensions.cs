using ElectronicVoting.Validator.Infrastructure.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace ElectronicVoting.Validator.Infrastructure.Startup;

public static class Extensions
{
    public static IServiceCollection AddStartupTasks(this IServiceCollection service)
    {
        service.AddHostedService<BlockchainSetupTask>();
        return service;
    }
}