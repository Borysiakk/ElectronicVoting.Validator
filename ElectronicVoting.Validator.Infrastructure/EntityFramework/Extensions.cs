using ElectronicVoting.Validator.Infrastructure.EntityFramework.Election;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.Election.Repositories;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework;

public static class Extensions
{
    public static IServiceCollection AddEntityFramework(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionStringForElection = configuration.GetConnectionString("ElectionDb");
        if (string.IsNullOrWhiteSpace(connectionStringForElection))
        {
            throw new Exception("Nie znaleziono connection string w konfiguracji dla ElectionDb");
        }
        
        var connectionStringForValidatorLedger= configuration.GetConnectionString("ValidatorDb");
        if (string.IsNullOrWhiteSpace(connectionStringForValidatorLedger))
        {
            throw new Exception("Nie znaleziono connection string w konfiguracji dla ElectionDb");
        }
        
        services.AddDbContext<ElectionDbContext>(options => options.UseNpgsql(connectionStringForElection));
        services.AddRepositoriesForElection();
        
        services.AddDbContext<ValidatorLedgerDbContext>(options => options.UseNpgsql(connectionStringForValidatorLedger));
        services.AddRepositoriesForValidatorLedger();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}