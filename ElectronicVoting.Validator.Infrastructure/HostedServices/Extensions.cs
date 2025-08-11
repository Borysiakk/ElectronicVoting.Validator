using Microsoft.Extensions.DependencyInjection;

namespace ElectronicVoting.Validator.Infrastructure.HostedServices;

public static class Extensions
{
    public static IServiceCollection AddHostedServices(this IServiceCollection service)
    {
        service.AddHostedService<PbftBlockCreatorService>();
        service.AddHostedService<VoteValidationTimeoutService>();
        return service;
    }
}