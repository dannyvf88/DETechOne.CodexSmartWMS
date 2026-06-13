using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Contracts.Dtos.Device;

public sealed class DeviceHeartbeatDto
{
    public string DeviceCode { get; init; } = string.Empty;
    public DeviceStatus Status { get; init; }
    public DateTime HeartbeatAtUtc { get; init; }
    public string? AppVersion { get; init; }
}
