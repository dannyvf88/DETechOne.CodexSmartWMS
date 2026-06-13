using DETechOne.SmartWMS.Application.Common;
using DETechOne.SmartWMS.Application.Security;
using DETechOne.SmartWMS.Contracts.Dtos.Auth;
using DETechOne.SmartWMS.Contracts.Requests.Auth;
using DETechOne.SmartWMS.Contracts.Responses.Auth;
using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Infrastructure.Configuration.Security;
using Microsoft.Extensions.Options;

namespace DETechOne.SmartWMS.Infrastructure.Security;

public sealed class InMemoryAuthService : IAuthService
{
    private const string SeedUserName = "admin";
    private const string SeedPassword = "SmartWMS#2026";

    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IClock _clock;
    private readonly JwtOptions _jwtOptions;
    private readonly string _seedPasswordHash;

    public InMemoryAuthService(
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IClock clock,
        IOptions<JwtOptions> jwtOptions)
    {
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _clock = clock;
        _jwtOptions = jwtOptions.Value;
        _seedPasswordHash = _passwordHasher.Hash(SeedPassword);
    }

    public Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Task.FromResult(Result<LoginResponse>.Fail("AUTH_REQUIRED", "Usuario y contraseña son requeridos."));
        }

        var isSeedUser = string.Equals(request.UserName.Trim(), SeedUserName, StringComparison.OrdinalIgnoreCase);
        if (!isSeedUser || !_passwordHasher.Verify(request.Password, _seedPasswordHash))
        {
            return Task.FromResult(Result<LoginResponse>.Fail("INVALID_CREDENTIALS", "Usuario o contraseña incorrectos."));
        }

        var user = new AuthUserDto
        {
            UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            UserName = SeedUserName,
            DisplayName = "SmartWMS Administrator",
            Email = "admin@detechone.local",
            CompanyCode = string.IsNullOrWhiteSpace(request.CompanyCode) ? "DEMO" : request.CompanyCode.Trim(),
            Roles = new[] { "Administrator" },
            Permissions = new[]
            {
                "smartwms.security.login",
                "smartwms.inventory.read",
                "smartwms.picking.manage",
                "smartwms.packing.manage",
                "smartwms.shipping.manage"
            }
        };

        var expiresAtUtc = _clock.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);
        var response = new LoginResponse
        {
            AccessToken = _jwtTokenService.CreateToken(user, expiresAtUtc),
            ExpiresAtUtc = expiresAtUtc,
            User = user
        };

        return Task.FromResult(Result<LoginResponse>.Ok(response, "Login correcto."));
    }
}
