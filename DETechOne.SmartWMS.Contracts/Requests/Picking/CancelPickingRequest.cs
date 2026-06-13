namespace DETechOne.SmartWMS.Contracts.Requests.Picking;

public sealed class CancelPickingRequest
{
    public Guid PickingId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
