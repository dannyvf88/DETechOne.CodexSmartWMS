namespace DETechOne.SmartWMS.Contracts.Requests.Movement;

public sealed class CancelMovementRequest
{
    public Guid MovementId { get; init; }
    public string Reason { get; init; } = string.Empty;
}
