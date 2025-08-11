using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Xunit;

namespace ElectronicVoting.Validator.Test.IntegrationTests;

public class ValidatorInstance: IAsyncLifetime
{
    public string PrivateKey { get; private set; }
    public ushort ApiHostPort { get; private set; }
    public string ValidatorName { get; private set; }
    public IContainer ApiContainer { get; private set; }
    public RabbitMqContainer RabbitMqContainer { get; private set; }
    public PostgreSqlContainer ValidatorDbContainer { get; private set; }
    public ValidatorInstance(string validatorName, ushort hostPort, string privateKey, string caCertificate, INetwork network, IFutureDockerImage image)
    {
        PrivateKey = privateKey;
        ApiHostPort = hostPort;
        ValidatorName = validatorName;
        
        var validatorDbAlias = $"{validatorName}-db";
        var rabbitMqAlias = $"{validatorName}-mq";
        
        ValidatorDbContainer = new PostgreSqlBuilder()
            .WithDatabase(validatorName)
            .WithUsername("user")
            .WithPassword("pass")
            .WithNetwork(network)
            .WithNetworkAliases(validatorDbAlias)
            .Build();
        
        RabbitMqContainer = new RabbitMqBuilder()
            .WithUsername("guest")
            .WithPassword("guest")
            .WithNetwork(network)
            .WithNetworkAliases(rabbitMqAlias)
            .WithPortBinding(0, 5672)
            .Build();
        
        ApiContainer = new ContainerBuilder()
            .WithImage(image)
            .WithNetwork(network)
            .WithPortBinding(hostPort, 8080)
            .WithEnvironment("RabbitMQ__Host", rabbitMqAlias)
            .WithEnvironment("RabbitMQ__Port", "5672")
            .WithEnvironment("RabbitMQ__Username", "guest")
            .WithEnvironment("RabbitMQ__Password", "guest")
            
            .WithEnvironment("Validator__CaCertification", caCertificate)
            .WithEnvironment("Validator__PrivateKeyForVoteSigning", privateKey)
            .WithEnvironment("ConnectionStrings__ElectionDb", "Host=election-db;Port=5432;Database=election;Username=user;Password=pass")
            .WithEnvironment("ConnectionStrings__ValidatorDb", $"Host={validatorDbAlias};Port=5432;Database={validatorName};Username=user;Password=pass")
            .WithEnvironment("Validator__Name", validatorName)
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
            .WithEnvironment("ASPNETCORE_URLS", "http://+:8080")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8080))
            .WithNetworkAliases(validatorName)
            .WithName(validatorName)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await ValidatorDbContainer.StartAsync(CancellationToken.None);
        await RabbitMqContainer.StartAsync(CancellationToken.None);
        await ApiContainer.StartAsync(CancellationToken.None);
    }

    public async Task DisposeAsync()
    {
        await ApiContainer.StopAsync();
        await ValidatorDbContainer.StopAsync();
        await RabbitMqContainer.StopAsync();
    }
}