using Microsoft.EntityFrameworkCore;

namespace ElectronicVoting.Validator.Test;

public static class DbContextFactory
{
    public static T Create<T>(string connectionString) where T : DbContext
    {
            var options = new DbContextOptionsBuilder<T>()
                .UseNpgsql(connectionString)
                .Options;
            
            return CreateWithOptions<T>(options);
    }
    
    public static T CreateInMemory<T>(string? databaseName = null) where T : DbContext
    {
        databaseName ??= Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<T>()
            .UseInMemoryDatabase(databaseName)
            .EnableSensitiveDataLogging()
            .Options;

        return CreateWithOptions<T>(options);
    }
    
    private static T CreateWithOptions<T>(DbContextOptions<T> options)
        where T : DbContext
    {
        try
        {
            return (T?)Activator.CreateInstance(typeof(T), options) ?? throw new InvalidOperationException("Nie można utworzyć DbContext");
        }
        catch (Exception)
        {
            Console.WriteLine("Konstruktor potrzebuje dodatkowego parametrow");
        }
        
        return (T?)Activator.CreateInstance(typeof(T), new object?[] { options, null })
               ?? throw new InvalidOperationException("Nie można utworzyć DbContext (options + env constructor zwrócił null).");
    }

}