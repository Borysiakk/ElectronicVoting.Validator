using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace ElectronicVoting.Validator.Test.IntegrationTests;

public static class DbContextFactory
{
    public static T Create<T>(string connectionString)
    where T : DbContext
    {
        var options = new DbContextOptionsBuilder<T>()
            .UseNpgsql(connectionString)
            .Options;

        try
        {
            return (T?)Activator.CreateInstance(typeof(T), options)
                   ?? throw new InvalidOperationException("Nie mozna utworzyc DbContext");
        }
        catch (Exception)
        {
            Console.WriteLine("Kontruktor potrzebuje dodatkowego parametru");
        }
        
        return (T?)Activator.CreateInstance(typeof(T), new object?[] { options, null})
               ?? throw new InvalidOperationException("Nie mozna utworzyc DbContext (options + env constructor zwrócił null).");

    }
}