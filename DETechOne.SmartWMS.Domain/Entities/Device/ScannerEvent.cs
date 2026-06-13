using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Domain.Entities.Device;

public sealed class ScannerEvent : BaseEntity
{
    private ScannerEvent()
    {
        Value = string.Empty;
    }

    public ScannerEvent(
        Guid scannerSessionId,
        ScannerEventType eventType,
        string value,
        string? symbology,
        string? source,
        string createdBy)
    {
        ScannerSessionId = scannerSessionId == Guid.Empty ? throw new ArgumentException("Scanner session id is required.", nameof(scannerSessionId)) : scannerSessionId;
        EventType = eventType;
        Value = string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Scan value is required.", nameof(value)) : value.Trim();
        Symbology = string.IsNullOrWhiteSpace(symbology) ? null : symbology.Trim();
        Source = string.IsNullOrWhiteSpace(source) ? null : source.Trim();
        ScannedAtUtc = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public Guid ScannerSessionId { get; private set; }
    public ScannerEventType EventType { get; private set; }
    public string Value { get; private set; }
    public string? Symbology { get; private set; }
    public string? Source { get; private set; }
    public DateTime ScannedAtUtc { get; private set; }
}
