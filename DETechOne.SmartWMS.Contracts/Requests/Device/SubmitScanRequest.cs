using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Contracts.Requests.Device;

public sealed class SubmitScanRequest
{
    public Guid ScannerSessionId { get; init; }
    public ScannerEventType EventType { get; init; } = ScannerEventType.DataWedgeIntent;
    public string Value { get; init; } = string.Empty;
    public string? Symbology { get; init; }
    public string? Source { get; init; } = "DataWedge";
}
