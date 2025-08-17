using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using ElectronicVoting.Validator.Domain.Entities.Election;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.Election;
using ElectronicVoting.Validator.Infrastructure.Minio;
using ElectronicVoting.Validator.Test.CertificateTools;
using Testcontainers.Minio;
using Testcontainers.PostgreSql;
using Xunit;

namespace ElectronicVoting.Validator.Test.IntegrationTests;

[CollectionDefinition("ValidatorFactory")]
public class ValidatorFactoryMarked : ICollectionFixture<ValidatorFactory>
{
    
}

public class ValidatorFactory :IAsyncLifetime
{
    private ElectionDbContext _electionDbContext = null!;
    
    private ushort _hostPort = 8079;
    private readonly CertificateKey _caCertificate;
    
    private readonly INetwork _network;
    private readonly IFutureDockerImage _image;
    private readonly MinioContainer _minioContainer;
    private readonly PostgreSqlContainer _electionDatabase;
    private readonly List<ValidatorInstance> _instanceValidators = new();
    
    public ValidatorFactory()
    {
        _caCertificate = CertificateGenerator.GenerateRootCaPemPair("CN=Ca, O=MyOrg, C=PL");
        
        _network = new NetworkBuilder()
            .WithName("election-network")
            .Build();
        
        _image = new ImageFromDockerfileBuilder()
            .WithDeleteIfExists(true)
            .WithName("electronicvoting.validator:dev")
            .WithDockerfileDirectory("/Users/borysiak/RiderProjects/ElectronicVoting.Validator")
            .WithDockerfile("Dockerfile")
            .Build();
        
        _electionDatabase = new PostgreSqlBuilder()
            .WithDatabase("election")
            .WithUsername("user")
            .WithPassword("pass")
            .WithNetwork(_network)
            .WithNetworkAliases("election-db")
            .Build();
        
        _minioContainer = new MinioBuilder()
            .WithImage("minio/minio:latest")
            .WithNetwork(_network)
            .WithNetworkAliases("minio")
            .WithEnvironment("MINIO_ROOT_USER","MINIO_ACCESS_KEY")
            .WithEnvironment("MINIO_ROOT_PASSWORD", "MINIO_SECRET_KEY")
            .Build();
    }

    public async Task<ValidatorInstance> Create(string validatorName, bool isLeader = false)
    {
        var certification = CertificateGenerator.GenerateCrtPemPair("CN=apiA, O=MyOrg, C=PL", _caCertificate.PrivateKeyRaw, _caCertificate.PublicKey);
        
        _hostPort++;
        var instance = new ValidatorInstance(
            validatorName,
            _hostPort,
            certification.PrivateKey,
            _caCertificate.PublicKey,
            _network,
            _image
        );
        
        _instanceValidators.Add(instance);
        await CreateValidatorNodeForElectionDatabase(validatorName, certification.PublicKey  , $"http://{validatorName}:{8080}", isLeader);;
        
        return instance;
    }

    public async Task InitializeAsync()
    {
        await _image.CreateAsync(CancellationToken.None);
        await _network.CreateAsync(CancellationToken.None);
        await _electionDatabase.StartAsync(CancellationToken.None);
        await _minioContainer.StartAsync(CancellationToken.None);
        
        _electionDbContext = DbContextFactory.Create<ElectionDbContext>(_electionDatabase.GetConnectionString());
        await _electionDbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        foreach (var instance in _instanceValidators)
        {
            try
            {
                if (instance.ApiContainer.State == TestcontainersStates.Running)
                    await instance.DisposeAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[WARN] Nie mozna usunac kontenera {instance.ValidatorName}, możliwe że został zwolniony recznie!");
            }
        }
        
        await _electionDatabase.StopAsync();
        await _network.DeleteAsync();
        await _image.DeleteAsync();
    }

    private async Task CreateValidatorNodeForElectionDatabase(string validatorName, string publicKey, string serverUrl, bool isLeader = false)
    {
        try
        {
            var node = new ValidatorNode()
            {
                Id = Guid.NewGuid(),
                Name = validatorName,
                PublicKey = publicKey,
                ServerUrl = serverUrl,
                IsLeader = isLeader,
            };
            await _electionDbContext.ValidatorNodes.AddAsync(node);
            await _electionDbContext.SaveChangesAsync();
            var firstOrDefault = _electionDbContext.ValidatorNodes.FirstOrDefault(a => a.Id == node.Id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}