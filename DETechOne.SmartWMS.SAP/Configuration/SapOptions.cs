namespace DETechOne.SmartWMS.SAP.Configuration;

public sealed class SapOptions
{
    public const string SectionName = "SAP";

    public string Mode { get; init; } = "Disabled";
    public string Server { get; init; } = string.Empty;
    public string CompanyDb { get; init; } = string.Empty;
    public string DbServerType { get; init; } = string.Empty;
    public string DbUserName { get; init; } = string.Empty;
    public string DbPassword { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string LicenseServer { get; init; } = string.Empty;
    public string ServiceLayerBaseUrl { get; init; } = string.Empty;
    public int ServiceLayerTimeoutSeconds { get; init; } = 100;
    public bool TrustServiceLayerCertificate { get; init; }

    public bool IsConfigured => !string.IsNullOrWhiteSpace(CompanyDb) && (!string.IsNullOrWhiteSpace(ServiceLayerBaseUrl) || !string.Equals(Mode, "ServiceLayer", StringComparison.OrdinalIgnoreCase));
}
