using DETechOne.SmartWMS.Web.Authentication;
using DETechOne.SmartWMS.Web.Models.Auth;
using DETechOne.SmartWMS.Web.Models.Common;
using DETechOne.SmartWMS.Web.Services.Api;
using Microsoft.AspNetCore.Components.Authorization;

namespace DETechOne.SmartWMS.Web.Services.Auth;

public sealed class AuthService(
    ISmartWmsApiClient apiClient,
    IAuthTokenStore tokenStore,
    AuthenticationStateProvider authenticationStateProvider) : IAuthService
{
    public async Task<bool> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = await apiClient.PostAsync<LoginRequest, ApiResponse<LoginResponse>>("api/auth/login", request, cancellationToken).ConfigureAwait(false);
        var payload = response?.GetPayload();
        var token = payload?.GetToken();

        if (response is null || !response.Success || string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        await tokenStore.SetTokenAsync(token, cancellationToken).ConfigureAwait(false);

        if (authenticationStateProvider is SmartWmsAuthenticationStateProvider provider)
        {
            provider.NotifyUserAuthenticationChanged();
        }

        return true;
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        await tokenStore.ClearTokenAsync(cancellationToken).ConfigureAwait(false);

        if (authenticationStateProvider is SmartWmsAuthenticationStateProvider provider)
        {
            provider.NotifyUserAuthenticationChanged();
        }
    }
}
