using System.Net.Http.Json;

namespace DETechOne.SmartWMS.Web.Services.Api;

public sealed class SmartWmsApiClient(IHttpClientFactory httpClientFactory) : ISmartWmsApiClient
{
    private const string ClientName = "SmartWmsApi";

    public async Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(ClientName);
        return await client.GetFromJsonAsync<TResponse>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(ClientName);
        var response = await client.PostAsJsonAsync(endpoint, request, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken).ConfigureAwait(false);
    }
}
