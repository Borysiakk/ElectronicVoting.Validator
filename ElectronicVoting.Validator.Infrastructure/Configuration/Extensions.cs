using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElectronicVoting.Validator.Infrastructure.Configuration;

public static class Extensions
{
    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ConsensusOptions>(configuration.GetSection(ConsensusOptions.SectionName));
        services.Configure<BlockValidationOptions>(configuration.GetSection(BlockValidationOptions.SectionName));
        services.Configure<BackgroundProcessOptions>(configuration.GetSection(BackgroundProcessOptions.SectionName));
        
        return services;
    }
}