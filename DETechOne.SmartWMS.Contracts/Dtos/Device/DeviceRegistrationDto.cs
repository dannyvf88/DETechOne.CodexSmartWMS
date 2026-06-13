using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Contracts.Dtos.Device;

public sealed class DeviceRegistrationDto
{
    public string DeviceCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public DeviceType DeviceType { get; init; }
    public string Model { get; init; } = string.Empty;
    public string Platform { get; init; } = string.Empty;
    public string AppVersion { get; init; } = string.Empty;
    public string? SerialNumber { get; init; }
    public string? WarehouseCode { get; init; }
    public DeviceStatus Status { get; init; }
    public DateTime? LastHeartbeatAtUtc { get; init; }
    public string? LastIpAddress { get; init; }
    public string? LastUserName { get; init; }
}
