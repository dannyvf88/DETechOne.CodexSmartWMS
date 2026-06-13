using DETechOne.SmartWMS.Contracts.Dtos.Audit;
using DETechOne.SmartWMS.Domain.Entities.Audit;

namespace DETechOne.SmartWMS.Application.Audit;

internal static class AuditMapper
{
    public static AuditLogEntryDto ToDto(AuditLogEntry entry) => new()
    {
        Id = entry.Id,
        Module = entry.Module,
        Action = entry.Action,
        EntityType = entry.EntityType,
        EntityId = entry.EntityId,
        UserName = entry.UserName,
        DeviceCode = entry.DeviceCode,
        CorrelationId = entry.CorrelationId,
        Description = entry.Description,
        Payload = entry.Payload,
        OccurredAtUtc = entry.OccurredAtUtc
    };
}
