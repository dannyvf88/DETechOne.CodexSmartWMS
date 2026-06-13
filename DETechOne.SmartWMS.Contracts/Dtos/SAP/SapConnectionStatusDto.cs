namespace DETechOne.SmartWMS.Contracts.Dtos.SAP;

public sealed class SapConnectionStatusDto
{
    public bool IsConfigured { get; init; }
    public bool IsConnected { get; init; }
    public string Mode { get; init; } = string.Empty;
    public string CompanyDb { get; init; } = string.Empty;
    public string Server { get; init; } = string.Empty;
    public string ServiceLayerBaseUrl { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}
