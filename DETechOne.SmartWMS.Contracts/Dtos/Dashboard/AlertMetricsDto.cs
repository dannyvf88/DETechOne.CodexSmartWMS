namespace DETechOne.SmartWMS.Contracts.Dtos.Dashboard;

public sealed class AlertMetricsDto
{
    public int Open { get; init; }
    public int Acknowledged { get; init; }
    public int Resolved { get; init; }
    public int Critical { get; init; }
    public int Error { get; init; }
    public int Warning { get; init; }
}
