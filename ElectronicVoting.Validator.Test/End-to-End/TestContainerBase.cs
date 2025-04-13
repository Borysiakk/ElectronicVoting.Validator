using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace ElectronicVoting.Validator.Test.End_to_End;

public class TestContainerBase: IAsyncLifetime
{
    protected readonly INetwork TestNetwork;
    
    protected readonly IContainer ApiContainer1;
    protected readonly IContainer ApiContainer2;
    
    protected readonly RedisContainer RedisContainer1;
    protected readonly RedisContainer RedisContainer2;
    
    protected readonly MsSqlContainer DatabaseContainer1;
    protected readonly MsSqlContainer DatabaseContainer2;

    public TestContainerBase()
    {
        TestNetwork = new NetworkBuilder()
            .WithName("electronic-testing-network" + Guid.NewGuid().ToString())
            .Build();
        
        RedisContainer1 = new RedisBuilder()
            .WithCleanUp(true)
            .WithImage("redis:latest")
            .WithNetwork(TestNetwork) 
            .WithNetworkAliases("electronicvoting.validator.redis.1") 
            .WithPortBinding(5004, 6379)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6379))
            .Build();
        
        DatabaseContainer1 = new MsSqlBuilder()
            .WithCleanUp(true)
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("LitwoOjczyznoMoja1234@")
            .WithNetwork(TestNetwork)
            .WithNetworkAliases("electronicvoting.validator.database.1")
            .WithPortBinding(5003, 1433) 
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
            .Build();
        
        ApiContainer1 = new ContainerBuilder()
            .WithImage("base_final:latest") 
            .WithPortBinding(5001, 80) 
            .WithPortBinding(5002, 443)
            .WithNetwork(TestNetwork)
            .WithEnvironment("ASPNETCORE_URLS", "https://+:443;http://+:80")
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development-Docker-Test")
            .WithEnvironment("CONTAINER_NAME", "electronicvoting.validator.api.1")
            .WithEnvironment("REDIS_URL", "electronicvoting.validator.redis.1:6379")
            .WithEnvironment("DATABASE_CONNECTION_STRING", "Server=electronicvoting.validator.database.1;User Id=sa;Password=LitwoOjczyznoMoja1234@;TrustServerCertificate=true")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(443))
            .Build();
        
        RedisContainer2 = new RedisBuilder()
            .WithCleanUp(true)
            .WithImage("redis:latest")
            .WithNetwork(TestNetwork) 
            .WithNetworkAliases("electronicvoting.validator.redis.2") 
            .WithPortBinding(5008, 6379)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6379))
            .Build();
        
        DatabaseContainer2 = new MsSqlBuilder()
            .WithCleanUp(true)
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("LitwoOjczyznoMoja1234@")
            .WithNetwork(TestNetwork)
            .WithNetworkAliases("electronicvoting.validator.database.2")
            .WithPortBinding(5007, 1433) 
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
            .Build();
        
        ApiContainer2 = new ContainerBuilder()
            .WithImage("base_final:latest") 
            .WithPortBinding(5005, 80) 
            .WithPortBinding(5006, 443)
            .WithNetwork(TestNetwork)
            .WithEnvironment("ASPNETCORE_URLS", "https://+:443;http://+:80")
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development-Docker-Test")
            .WithEnvironment("CONTAINER_NAME", "electronicvoting.validator.api.2")
            .WithEnvironment("REDIS_URL", "electronicvoting.validator.redis.2:6379")
            .WithEnvironment("DATABASE_CONNECTION_STRING", "Server=electronicvoting.validator.database.2;User Id=sa;Password=LitwoOjczyznoMoja1234@;TrustServerCertificate=true")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(443))
            .Build();
    }
    
    public async Task InitializeAsync()
    {
        await RedisContainer1.StartAsync();
        await RedisContainer2.StartAsync();
        
        await DatabaseContainer1.StartAsync();
        await DatabaseContainer2.StartAsync();
        
        await ApiContainer1.StartAsync();
        await ApiContainer2.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await ApiContainer1.StopAsync();
        await ApiContainer2.StopAsync();
        
        await DatabaseContainer1.StopAsync();
        await DatabaseContainer2.StopAsync();
        
        await RedisContainer1.StopAsync();
        await RedisContainer2.StopAsync();
        
        await TestNetwork.DisposeAsync();
        
        await ApiContainer1.DisposeAsync();
        await ApiContainer2.DisposeAsync();
        
        await DatabaseContainer1.DisposeAsync();
        await DatabaseContainer2.DisposeAsync();
        
        await RedisContainer1.DisposeAsync();
        await RedisContainer2.DisposeAsync();
    }
}