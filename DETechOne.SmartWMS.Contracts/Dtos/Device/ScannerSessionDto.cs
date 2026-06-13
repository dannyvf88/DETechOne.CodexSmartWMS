using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Contracts.Dtos.Device;

public sealed class ScannerSessionDto
{
    public Guid Id { get; init; }
    public string DeviceCode { get; init; } = string.Empty;
    public string Operation { get; init; } = string.Empty;
    public string? ReferenceDocument { get; init; }
    public string OperatorUserName { get; init; } = string.Empty;
    public ScannerSessionStatus Status { get; init; }
    public DateTime StartedAtUtc { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
    public DateTime? CancelledAtUtc { get; init; }
    public string? CancellationReason { get; init; }
    public IReadOnlyCollection<ScannerEventDto> Events { get; init; } = Array.Empty<ScannerEventDto>();
}
