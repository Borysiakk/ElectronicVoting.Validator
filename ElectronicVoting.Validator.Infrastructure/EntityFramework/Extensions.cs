using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework;

public static class Extensions
{
    public static IServiceCollection AddEntityFramework(this IServiceCollection service)
    {
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new Exception("Error while reading the connection string from the database.");

        service.AddDbContext<ValidatorDbContext>(option => option.UseSqlServer(connectionString));
        service.AddTransient<IUnitOfWork, UnitOfWork>();

        return service;
    }
}