namespace DETechOne.SmartWMS.Contracts.Dtos.Alerts;

public sealed class OperationalAlertDto
{
    public Guid Id { get; init; }
    public string Severity { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Source { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string? EntityType { get; init; }
    public string? EntityId { get; init; }
    public string? UserName { get; init; }
    public string? DeviceCode { get; init; }
    public string? CorrelationId { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? AcknowledgedAtUtc { get; init; }
    public string? AcknowledgedBy { get; init; }
    public DateTime? ResolvedAtUtc { get; init; }
    public string? ResolvedBy { get; init; }
}
