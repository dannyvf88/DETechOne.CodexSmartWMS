namespace DETechOne.SmartWMS.Contracts.Dtos.Dashboard;

public sealed class DashboardOverviewDto
{
    public DateTime GeneratedAtUtc { get; init; }
    public DateTime? FromUtc { get; init; }
    public DateTime? ToUtc { get; init; }
    public string? WarehouseCode { get; init; }
    public OperationStatusMetricsDto Picking { get; init; } = new();
    public OperationStatusMetricsDto Packing { get; init; } = new();
    public OperationStatusMetricsDto Shipping { get; init; } = new();
    public DeviceMetricsDto Devices { get; init; } = new();
    public AlertMetricsDto Alerts { get; init; } = new();
    public IReadOnlyCollection<MetricCounterDto> Counters { get; init; } = Array.Empty<MetricCounterDto>();
}
