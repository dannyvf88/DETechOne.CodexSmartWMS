using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Contracts.Dtos.Device;

public sealed class ScannerEventDto
{
    public Guid Id { get; init; }
    public Guid ScannerSessionId { get; init; }
    public ScannerEventType EventType { get; init; }
    public string Value { get; init; } = string.Empty;
    public string? Symbology { get; init; }
    public string? Source { get; init; }
    public DateTime ScannedAtUtc { get; init; }
}
