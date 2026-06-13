using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Contracts.Requests.Device;

public sealed class RegisterDeviceRequest
{
    public string DeviceCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public DeviceType DeviceType { get; init; } = DeviceType.ZebraTc15;
    public string Model { get; init; } = string.Empty;
    public string Platform { get; init; } = "Android";
    public string AppVersion { get; init; } = string.Empty;
    public string? SerialNumber { get; init; }
    public string? WarehouseCode { get; init; }
    public string? IpAddress { get; init; }
}
