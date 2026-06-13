namespace DETechOne.SmartWMS.Infrastructure.Configuration.Database;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public string Provider { get; set; } = "SqlServer";
    public string ConnectionString { get; set; } = string.Empty;
    public int CommandTimeoutSeconds { get; set; } = 30;
    public bool EnableSensitiveDataLogging { get; set; }

    public string HanaProviderInvariantName { get; set; } = "Sap.Data.Hana";

    public bool IsConfigured => !string.IsNullOrWhiteSpace(ConnectionString);
}
