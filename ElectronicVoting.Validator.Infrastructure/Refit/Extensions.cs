using Microsoft.Extensions.DependencyInjection;

namespace ElectronicVoting.Validator.Infrastructure.Refit;

public static class Extensions
{
    public static void AddRafit(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddScoped<HttpApiClientFactory>();
    }
}