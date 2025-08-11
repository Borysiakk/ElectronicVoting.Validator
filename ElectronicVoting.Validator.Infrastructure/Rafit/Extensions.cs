using Microsoft.Extensions.DependencyInjection;

namespace ElectronicVoting.Validator.Infrastructure.Rafit;

public static class Extensions
{
    public static void AddRafit(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<IHttpApiClientFactory, HttpApiClientFactory>();
        services.AddHttpClient();
    }
}