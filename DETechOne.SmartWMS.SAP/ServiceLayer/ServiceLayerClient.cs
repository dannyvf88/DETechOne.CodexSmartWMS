using System.Text.Json;
using DETechOne.SmartWMS.Application.SAP;
using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.SAP.Configuration;
using Microsoft.Extensions.Options;

namespace DETechOne.SmartWMS.SAP.ServiceLayer;

public sealed class ServiceLayerClient : IServiceLayerClient
{
    private readonly HttpClient _httpClient;
    private readonly SapOptions _options;

    public ServiceLayerClient(HttpClient httpClient, IOptions<SapOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<Result<string>> GetAsync(string relativeUrl, CancellationToken cancellationToken = default)
    {
        if (!CanExecute(out string error))
        {
            return Result<string>.Fail("SAP_NOT_CONFIGURED", error);
        }

        using HttpResponseMessage response = await _httpClient.GetAsync(NormalizeRelativeUrl(relativeUrl), cancellationToken);
        string content = await response.Content.ReadAsStringAsync(cancellationToken);

        return response.IsSuccessStatusCode
            ? Result<string>.Ok(content)
            : Result<string>.Fail("SAP_SERVICE_LAYER_ERROR", content);
    }

    public async Task<Result<string>> PostAsync(string relativeUrl, object payload, CancellationToken cancellationToken = default)
    {
        if (!CanExecute(out string error))
        {
            return Result<string>.Fail("SAP_NOT_CONFIGURED", error);
        }

        using StringContent body = new(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
        using HttpResponseMessage response = await _httpClient.PostAsync(NormalizeRelativeUrl(relativeUrl), body, cancellationToken);
        string content = await response.Content.ReadAsStringAsync(cancellationToken);

        return response.IsSuccessStatusCode
            ? Result<string>.Ok(content)
            : Result<string>.Fail("SAP_SERVICE_LAYER_ERROR", content);
    }

    private bool CanExecute(out string error)
    {
        if (string.IsNullOrWhiteSpace(_options.ServiceLayerBaseUrl))
        {
            error = "SAP:ServiceLayerBaseUrl is empty.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    private static string NormalizeRelativeUrl(string relativeUrl)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativeUrl);
        return relativeUrl.TrimStart('/');
    }
}
