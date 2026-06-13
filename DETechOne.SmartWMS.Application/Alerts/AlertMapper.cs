using DETechOne.SmartWMS.Contracts.Dtos.Alerts;
using DETechOne.SmartWMS.Domain.Entities.Alerts;

namespace DETechOne.SmartWMS.Application.Alerts;

internal static class AlertMapper
{
    public static OperationalAlertDto ToDto(OperationalAlert alert) => new()
    {
        Id = alert.Id,
        Severity = alert.Severity.ToString(),
        Status = alert.Status.ToString(),
        Source = alert.Source,
        Code = alert.Code,
        Title = alert.Title,
        Message = alert.Message,
        EntityType = alert.EntityType,
        EntityId = alert.EntityId,
        UserName = alert.UserName,
        DeviceCode = alert.DeviceCode,
        CorrelationId = alert.CorrelationId,
        CreatedAtUtc = alert.CreatedAtUtc,
        AcknowledgedAtUtc = alert.AcknowledgedAtUtc,
        AcknowledgedBy = alert.AcknowledgedBy,
        ResolvedAtUtc = alert.ResolvedAtUtc,
        ResolvedBy = alert.ResolvedBy
    };
}
