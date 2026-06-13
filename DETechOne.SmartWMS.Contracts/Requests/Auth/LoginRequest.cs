namespace DETechOne.SmartWMS.Contracts.Requests.Auth;

public sealed class LoginRequest
{
    public string UserName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? CompanyCode { get; init; }
}
