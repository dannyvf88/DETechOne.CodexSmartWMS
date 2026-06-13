namespace DETechOne.SmartWMS.Contracts.Requests.Movement;

public sealed class CreateMovementRequest
{
    public string MovementType { get; init; } = "Transfer";
    public string ReferenceType { get; init; } = string.Empty;
    public string? ReferenceNumber { get; init; }
    public IReadOnlyCollection<CreateMovementLineRequest> Lines { get; init; } = Array.Empty<CreateMovementLineRequest>();
}
