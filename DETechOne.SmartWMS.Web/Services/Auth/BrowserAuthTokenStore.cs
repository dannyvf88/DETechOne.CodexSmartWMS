using Microsoft.JSInterop;

namespace DETechOne.SmartWMS.Web.Services.Auth;

public sealed class BrowserAuthTokenStore(IJSRuntime jsRuntime) : IAuthTokenStore
{
    private const string TokenKey = "smartwms.auth.token";

    public async Task<string?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await jsRuntime.InvokeAsync<string?>("smartWmsStorage.get", cancellationToken, TokenKey);
        }
        catch (Exception ex) when (IsBrowserStorageUnavailable(ex))
        {
            return null;
        }
    }

    public async Task SetTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("smartWmsStorage.set", cancellationToken, TokenKey, token);
        }
        catch (Exception ex) when (IsBrowserStorageUnavailable(ex))
        {
        }
    }

    public async Task ClearTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("smartWmsStorage.remove", cancellationToken, TokenKey);
        }
        catch (Exception ex) when (IsBrowserStorageUnavailable(ex))
        {
        }
    }

    private static bool IsBrowserStorageUnavailable(Exception ex)
    {
        return ex is InvalidOperationException or JSException or JSDisconnectedException;
    }
}
