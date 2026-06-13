using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DETechOne.SmartWMS.Web.Services.Auth;
using Microsoft.AspNetCore.Components.Authorization;

namespace DETechOne.SmartWMS.Web.Authentication;

public sealed class SmartWmsAuthenticationStateProvider(IAuthTokenStore tokenStore) : AuthenticationStateProvider
{
    private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await tokenStore.GetTokenAsync().ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(Anonymous);
        }

        var identity = BuildIdentity(token);

        if (!identity.IsAuthenticated)
        {
            await tokenStore.ClearTokenAsync().ConfigureAwait(false);
            return new AuthenticationState(Anonymous);
        }

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void NotifyUserAuthenticationChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private static ClaimsIdentity BuildIdentity(string token)
    {
        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            if (jwt.ValidTo != DateTime.MinValue && jwt.ValidTo <= DateTime.UtcNow)
            {
                return new ClaimsIdentity();
            }

            var claims = NormalizeClaims(jwt.Claims);
            return new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
        }
        catch
        {
            return new ClaimsIdentity();
        }
    }

    private static IEnumerable<Claim> NormalizeClaims(IEnumerable<Claim> sourceClaims)
    {
        var claims = sourceClaims.ToList();

        var nameClaim = claims.FirstOrDefault(c =>
            c.Type is ClaimTypes.Name or JwtRegisteredClaimNames.UniqueName or JwtRegisteredClaimNames.Sub or "name" or "userName");

        if (nameClaim is not null && claims.All(c => c.Type != ClaimTypes.Name))
        {
            claims.Add(new Claim(ClaimTypes.Name, nameClaim.Value));
        }

        foreach (var roleClaim in claims.Where(c => c.Type is "role" or "roles").ToList())
        {
            if (claims.All(c => c.Type != ClaimTypes.Role || c.Value != roleClaim.Value))
            {
                claims.Add(new Claim(ClaimTypes.Role, roleClaim.Value));
            }
        }

        return claims;
    }
}
