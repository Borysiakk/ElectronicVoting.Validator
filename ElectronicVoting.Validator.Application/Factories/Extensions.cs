using Microsoft.Extensions.DependencyInjection;

namespace ElectronicVoting.Validator.Application.Factories;


public static class Extensions
{
    public static IServiceCollection AddFactories(this IServiceCollection service)
    {
        service.AddScoped<IPendingBlockFactory, PendingBlockFactory>();
        return service;
    }
}