namespace DETechOne.SmartWMS.Contracts.Requests.Audit;

public sealed class AuditLogQueryRequest
{
    public string? Module { get; init; }
    public string? Action { get; init; }
    public string? EntityType { get; init; }
    public string? EntityId { get; init; }
    public string? UserName { get; init; }
    public string? DeviceCode { get; init; }
    public string? CorrelationId { get; init; }
    public DateTime? FromUtc { get; init; }
    public DateTime? ToUtc { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}
