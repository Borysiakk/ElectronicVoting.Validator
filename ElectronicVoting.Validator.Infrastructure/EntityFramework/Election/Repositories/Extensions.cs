using Microsoft.Extensions.DependencyInjection;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.Election.Repositories;

public static class Extensions
{
    public static void AddRepositoriesForElection(this IServiceCollection services)
    {
        services.AddScoped<IValidatorNodeRepository, ValidatorNodeRepository>();
    }
}