namespace DETechOne.SmartWMS.Contracts.Dtos.Dashboard;

public sealed class DeviceMetricsDto
{
    public int Registered { get; init; }
    public int Online { get; init; }
    public int Offline { get; init; }
    public int Blocked { get; init; }
    public DateTime? LastHeartbeatAtUtc { get; init; }
}
