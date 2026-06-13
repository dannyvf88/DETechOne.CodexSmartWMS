namespace DETechOne.SmartWMS.Contracts.Requests.Alerts;

public sealed class AlertQueryRequest
{
    public string? Severity { get; init; }
    public string? Status { get; init; }
    public string? Source { get; init; }
    public string? Code { get; init; }
    public string? EntityType { get; init; }
    public string? EntityId { get; init; }
    public string? DeviceCode { get; init; }
    public DateTime? FromUtc { get; init; }
    public DateTime? ToUtc { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}
