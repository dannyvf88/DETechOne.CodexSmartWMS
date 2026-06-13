namespace DETechOne.SmartWMS.Contracts.Requests.Device;

public sealed class StartScannerSessionRequest
{
    public string DeviceCode { get; init; } = string.Empty;
    public string Operation { get; init; } = string.Empty;
    public string? ReferenceDocument { get; init; }
}
