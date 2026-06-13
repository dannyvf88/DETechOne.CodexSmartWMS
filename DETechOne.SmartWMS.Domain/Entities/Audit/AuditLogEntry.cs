using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Domain.Entities.Audit;

public sealed class AuditLogEntry : BaseEntity
{
    public AuditLogEntry(
        string module,
        string action,
        string entityType,
        string entityId,
        string userName,
        string? deviceCode,
        string? correlationId,
        string? description,
        string? payload,
        DateTime occurredAtUtc)
    {
        Id = Guid.NewGuid();
        Module = module;
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
        UserName = userName;
        DeviceCode = deviceCode;
        CorrelationId = correlationId;
        Description = description;
        Payload = payload;
        OccurredAtUtc = occurredAtUtc;
        CreatedAtUtc = occurredAtUtc;
    }

    public string Module { get; private set; }
    public string Action { get; private set; }
    public string EntityType { get; private set; }
    public string EntityId { get; private set; }
    public string UserName { get; private set; }
    public string? DeviceCode { get; private set; }
    public string? CorrelationId { get; private set; }
    public string? Description { get; private set; }
    public string? Payload { get; private set; }
    public DateTime OccurredAtUtc { get; private set; }
}
