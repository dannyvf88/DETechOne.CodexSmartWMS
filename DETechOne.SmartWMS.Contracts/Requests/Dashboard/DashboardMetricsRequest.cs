namespace DETechOne.SmartWMS.Contracts.Requests.Dashboard;

public sealed class DashboardMetricsRequest
{
    public DateTime? FromUtc { get; init; }
    public DateTime? ToUtc { get; init; }
    public string? WarehouseCode { get; init; }
}
