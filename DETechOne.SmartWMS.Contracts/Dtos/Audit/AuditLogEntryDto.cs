namespace DETechOne.SmartWMS.Contracts.Dtos.Audit;

public sealed class AuditLogEntryDto
{
    public Guid Id { get; init; }
    public string Module { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public string EntityId { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string? DeviceCode { get; init; }
    public string? CorrelationId { get; init; }
    public string? Description { get; init; }
    public string? Payload { get; init; }
    public DateTime OccurredAtUtc { get; init; }
}
