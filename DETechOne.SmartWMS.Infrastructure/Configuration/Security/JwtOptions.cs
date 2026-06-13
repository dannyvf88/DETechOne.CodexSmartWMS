namespace DETechOne.SmartWMS.Infrastructure.Configuration.Security;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "DETechOne.SmartWMS";
    public string Audience { get; init; } = "DETechOne.SmartWMS.Clients";
    public string SigningKey { get; init; } = "CHANGE_ME_DETechOne_SmartWMS_Development_Key_32_Chars";
    public int ExpirationMinutes { get; init; } = 120;
}
