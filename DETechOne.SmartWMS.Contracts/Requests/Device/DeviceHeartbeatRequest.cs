namespace DETechOne.SmartWMS.Contracts.Requests.Device;

public sealed class DeviceHeartbeatRequest
{
    public string DeviceCode { get; init; } = string.Empty;
    public string? AppVersion { get; init; }
    public string? IpAddress { get; init; }
}
