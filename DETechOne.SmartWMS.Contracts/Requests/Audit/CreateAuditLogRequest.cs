namespace DETechOne.SmartWMS.Contracts.Requests.Audit;

public sealed class CreateAuditLogRequest
{
    public string Module { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public string EntityId { get; init; } = string.Empty;
    public string? DeviceCode { get; init; }
    public string? CorrelationId { get; init; }
    public string? Description { get; init; }
    public string? Payload { get; init; }
}
