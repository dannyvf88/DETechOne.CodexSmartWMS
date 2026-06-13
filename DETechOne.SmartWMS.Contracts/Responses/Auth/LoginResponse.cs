using DETechOne.SmartWMS.Contracts.Dtos.Auth;

namespace DETechOne.SmartWMS.Contracts.Responses.Auth;

public sealed class LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string TokenType { get; init; } = "Bearer";
    public DateTime ExpiresAtUtc { get; init; }
    public AuthUserDto User { get; init; } = new();
}
