using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Domain.Entities.Alerts;

public sealed class OperationalAlert : BaseEntity
{
    public OperationalAlert(
        AlertSeverity severity,
        AlertStatus status,
        string source,
        string code,
        string title,
        string message,
        string? entityType,
        string? entityId,
        string? userName,
        string? deviceCode,
        string? correlationId,
        DateTime createdAtUtc)
    {
        Id = Guid.NewGuid();
        Severity = severity;
        Status = status;
        Source = source;
        Code = code;
        Title = title;
        Message = message;
        EntityType = entityType;
        EntityId = entityId;
        UserName = userName;
        DeviceCode = deviceCode;
        CorrelationId = correlationId;
        CreatedAtUtc = createdAtUtc;
    }

    public AlertSeverity Severity { get; private set; }
    public AlertStatus Status { get; private set; }
    public string Source { get; private set; }
    public string Code { get; private set; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public string? EntityType { get; private set; }
    public string? EntityId { get; private set; }
    public string? UserName { get; private set; }
    public string? DeviceCode { get; private set; }
    public string? CorrelationId { get; private set; }
    public DateTime? AcknowledgedAtUtc { get; private set; }
    public string? AcknowledgedBy { get; private set; }
    public DateTime? ResolvedAtUtc { get; private set; }
    public string? ResolvedBy { get; private set; }

    public void Acknowledge(string userName, DateTime acknowledgedAtUtc)
    {
        if (Status == AlertStatus.Resolved)
        {
            return;
        }

        Status = AlertStatus.Acknowledged;
        AcknowledgedBy = userName;
        AcknowledgedAtUtc = acknowledgedAtUtc;
        UpdatedAtUtc = acknowledgedAtUtc;
    }

    public void Resolve(string userName, DateTime resolvedAtUtc)
    {
        Status = AlertStatus.Resolved;
        ResolvedBy = userName;
        ResolvedAtUtc = resolvedAtUtc;
        UpdatedAtUtc = resolvedAtUtc;
    }
}
