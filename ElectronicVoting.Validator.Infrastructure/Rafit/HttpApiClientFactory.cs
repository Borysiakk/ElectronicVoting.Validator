using Microsoft.Extensions.Caching.Memory;
using Refit; 
namespace ElectronicVoting.Validator.Infrastructure.Rafit;

public interface IHttpApiClientFactory
{
    T CreateClient<T>(string baseUrl);
}

public class HttpApiClientFactory(IHttpClientFactory httpClientFactory, IMemoryCache cache) : IHttpApiClientFactory
{
    public T CreateClient<T>(string baseUrl)
    {
        var cacheKey = $"refit_{typeof(T).Name}_{baseUrl}";
        
        return cache.GetOrCreate(cacheKey, factory =>
        {
            factory.SetAbsoluteExpiration(TimeSpan.FromHours(24)); 
            
            var httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(baseUrl);
            return RestService.For<T>(httpClient);
        })!;
    }
}