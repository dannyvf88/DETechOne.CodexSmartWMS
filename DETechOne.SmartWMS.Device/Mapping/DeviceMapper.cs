using DETechOne.SmartWMS.Contracts.Dtos.Device;
using DETechOne.SmartWMS.Domain.Entities.Device;

namespace DETechOne.SmartWMS.Device.Mapping;

internal static class DeviceMapper
{
    public static DeviceRegistrationDto ToDto(WmsDevice device) => new()
    {
        DeviceCode = device.DeviceCode,
        Name = device.Name,
        DeviceType = device.DeviceType,
        Model = device.Model,
        Platform = device.Platform,
        AppVersion = device.AppVersion,
        SerialNumber = device.SerialNumber,
        WarehouseCode = device.WarehouseCode,
        Status = device.Status,
        LastHeartbeatAtUtc = device.LastHeartbeatAtUtc,
        LastIpAddress = device.LastIpAddress,
        LastUserName = device.LastUserName
    };

    public static ScannerSessionDto ToDto(ScannerSession session) => new()
    {
        Id = session.Id,
        DeviceCode = session.DeviceCode,
        Operation = session.Operation,
        ReferenceDocument = session.ReferenceDocument,
        OperatorUserName = session.OperatorUserName,
        Status = session.Status,
        StartedAtUtc = session.StartedAtUtc,
        CompletedAtUtc = session.CompletedAtUtc,
        CancelledAtUtc = session.CancelledAtUtc,
        CancellationReason = session.CancellationReason,
        Events = session.Events.Select(ToDto).ToArray()
    };

    public static ScannerEventDto ToDto(ScannerEvent scannerEvent) => new()
    {
        Id = scannerEvent.Id,
        ScannerSessionId = scannerEvent.ScannerSessionId,
        EventType = scannerEvent.EventType,
        Value = scannerEvent.Value,
        Symbology = scannerEvent.Symbology,
        Source = scannerEvent.Source,
        ScannedAtUtc = scannerEvent.ScannedAtUtc
    };
}
