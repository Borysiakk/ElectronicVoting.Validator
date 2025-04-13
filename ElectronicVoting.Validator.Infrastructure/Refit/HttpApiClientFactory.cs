using Refit;

namespace ElectronicVoting.Validator.Infrastructure.Refit;

public class HttpApiClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HttpApiClientFactory(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public T CreateClient<T>(string baseUrl)
    {
        var httpClient = _httpClientFactory.CreateClient("CustomClient");
        httpClient.BaseAddress = new Uri(baseUrl);
        
        return RestService.For<T>(httpClient);
    }
}