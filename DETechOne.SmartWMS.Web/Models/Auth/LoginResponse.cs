namespace DETechOne.SmartWMS.Web.Models.Auth;

public sealed class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public IReadOnlyCollection<string> Roles { get; set; } = Array.Empty<string>();

    public string GetToken() => !string.IsNullOrWhiteSpace(AccessToken) ? AccessToken : Token;
}
