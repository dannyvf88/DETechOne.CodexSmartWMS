namespace DETechOne.SmartWMS.Contracts.Requests.Packing;

public sealed class CancelPackingRequest
{
    public Guid PackingId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
