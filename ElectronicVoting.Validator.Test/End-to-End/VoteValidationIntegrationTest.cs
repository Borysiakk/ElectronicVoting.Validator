using System.Text;
using System.Text.Json;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using ElectronicVoting.Validator.Application.Handlers.Commands.Api.Election;
using ElectronicVoting.Validator.Domain.Models;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace ElectronicVoting.Validator.Test.End_to_End;

public class VoteValidationIntegrationTest: TestContainerBase
{
    [Fact]
    public async Task ShouldProcessVoteThroughPbftAndSaveToBlockchain()
    {
        var castVote = GenerateTestVote();
        var json = ToJson(castVote);
        var httpClient = InitializeHttpClient("https://localhost:5002");
        
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("cast-vote", content);
        
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        
        Assert.NotNull(responseContent);
        Assert.True(response.IsSuccessStatusCode);
    }

    private CastVote GenerateTestVote()
    {
        return new CastVote()
        {
            VoteEncryption = new VoteEncryption()
            {
                VoteEncryptionDetails = new VoteEncryptionDetails()
                {
                    R = 1,
                    Vote = "1"
                },
                VoteProofOfKnowledgeBase = new VoteProofOfKnowledgeBase()
                {
                    E = ["1"],
                    U = ["1"],
                    V = ["1"]
                }
            }
        };
    }
    
    private static string ToJson(CastVote castVote)
    {
        return JsonSerializer.Serialize(castVote, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    private HttpClient InitializeHttpClient(string host)
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };
        
        return new HttpClient(handler)
        {
            BaseAddress = new Uri(host) 
        };
    }
}