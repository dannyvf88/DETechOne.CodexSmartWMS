namespace DETechOne.SmartWMS.Contracts.Dtos.Movement;

public sealed class MovementDocumentDto
{
    public Guid Id { get; init; }
    public string MovementNumber { get; init; } = string.Empty;
    public string MovementType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string ReferenceType { get; init; } = string.Empty;
    public string? ReferenceNumber { get; init; }
    public string RequestedBy { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? ConfirmedAtUtc { get; init; }
    public string? ConfirmedBy { get; init; }
    public DateTime? CancelledAtUtc { get; init; }
    public string? CancelledBy { get; init; }
    public string? CancelReason { get; init; }
    public IReadOnlyCollection<MovementLineDto> Lines { get; init; } = Array.Empty<MovementLineDto>();
}
