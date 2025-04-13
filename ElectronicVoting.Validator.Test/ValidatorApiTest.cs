using System.Diagnostics;
using Docker.DotNet;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace ElectronicVoting.Validator.Test;


public class ValidatorApiTest
{
    private readonly INetwork _testNetwork;
    
    private readonly IContainer _apiContainer1;
    private readonly RedisContainer _redisContainer1;
    private readonly MsSqlContainer _datebaseContainer1;
    
    public ValidatorApiTest()
    {
        const long TwoGB = 2L * 1024 * 1024 * 1024;
        
        // Tworzenie sieci dla kontenerów
        _testNetwork = new NetworkBuilder()
            .WithName("electronic-testing-network.10")
            .Build();
        
        _redisContainer1 = new RedisBuilder()
            .WithImage("redis:latest") // Wybór obrazu Redis
            .WithNetwork(_testNetwork) // Przydzielenie do tej samej sieci
            .WithNetworkAliases("electronicvoting.validator.redis.1") // Alias dla Redis
            .WithPortBinding(5004, 6379) // Mapowanie portu
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6379))
            .Build();
        
        _datebaseContainer1 = new MsSqlBuilder()
            .WithCleanUp(true)
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("LitwoOjczyznoMoja1234@")
            .WithNetwork(_testNetwork) // Dodanie kontenera do wspólnej sieci
            .WithNetworkAliases("electronicvoting.validator.database.1") // Alias dla bazy w sieci
            .WithPortBinding(5003, 1433) // Mapowanie MSSQL na odpowiedni port
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
            .Build();
        
        _apiContainer1 = new ContainerBuilder()
            .WithImage("base_final:latest") // obraz pierwszego API
            .WithPortBinding(5001, 80) // HTTP
            .WithPortBinding(5002, 443) // HTTPS
            .WithNetwork(_testNetwork)
            .WithEnvironment("ASPNETCORE_URLS", "https://+:443;http://+:80")
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development-Docker-Test")
            .WithEnvironment("CONTAINER_NAME", "electronicvoting.validator.api.1")
            .WithEnvironment("REDIS_URL", "electronicvoting.validator.redis.1:6379")
            .WithEnvironment("DATABASE_CONNECTION_STRING", "Server=electronicvoting.validator.database.1;User Id=sa;Password=LitwoOjczyznoMoja1234@;TrustServerCertificate=true")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(443))
            .Build();
        
        // _redisContainer2 = new RedisBuilder()
        //     .WithImage("redis:latest") // Wybór obrazu Redis
        //     .WithNetwork(_testNetwork) // Przydzielenie do tej samej sieci
        //     .WithNetworkAliases("electronicvoting.validator.redis.1") // Alias dla Redis
        //     .WithPortBinding(5008, 6379) // Mapowanie portu
        //     .Build();
        //
        // _datebaseContainer1 = new MsSqlBuilder()
        //     .WithCleanUp(true)
        //     .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        //     .WithPassword("Your_password123")
        //     .WithNetwork(_testNetwork) // Dodanie kontenera do wspólnej sieci
        //     .WithNetworkAliases("electronicvoting.validator.database.1") // Alias dla bazy w sieci
        //     .WithPortBinding(5007, 1433) // Mapowanie MSSQL na odpowiedni port
        //     .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
        //     .Build();
        //
        // _apiContainer2 = new ContainerBuilder()
        //     .WithImage("electronicvoting.validator.api.1:dev") // obraz pierwszego API
        //     .WithPortBinding(5005, 80) // HTTP
        //     .WithPortBinding(5006, 443) // HTTPS
        //     .WithEnvironment("ASPNETCORE_URLS", "https://+:443;http://+:80")
        //     .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
        //     .WithEnvironment("CONTAINER_NAME", "electronicvoting.validator.api.1")
        //     .WithEnvironment("REDIS_URL", "electronicvoting.validator.redis.1:6379")
        //     .WithEnvironment("DATABASE_CONNECTION_STRING", "Server=electronicvoting.validator.database.1;User Id=sa;Password=LitwoOjczyznoMoja1234@;TrustServerCertificate=true")
        //     .WithNetwork(_testNetwork)
        //     .Build();
    }
    
    [Fact]
    public async Task Test()
    {
        try
        {
            await _redisContainer1.StartAsync();
            await _datebaseContainer1.StartAsync();
            await _apiContainer1.StartAsync();
            
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:5002") 
            };
            
            var response = await httpClient.GetAsync("api/Test/Test");
            response.EnsureSuccessStatusCode(); // Weryfikacja czy odpowiedź była poprawna (kod 2xx)
            var resultString = await response.Content.ReadAsStringAsync();
            var result = int.Parse(resultString);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            await _apiContainer1.StopAsync();
            await _apiContainer1.DisposeAsync();
            
            await _datebaseContainer1.StopAsync();
            await _datebaseContainer1.DisposeAsync();
            
            await _redisContainer1.StopAsync();
            await _redisContainer1.DisposeAsync();
            
            await _testNetwork.DeleteAsync();
            await _testNetwork.DisposeAsync();
            
        }
    }
}