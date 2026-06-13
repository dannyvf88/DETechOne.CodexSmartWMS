namespace DETechOne.SmartWMS.Contracts.Requests.Packing;

public sealed class PackItemRequest
{
    public Guid PackingId { get; set; }
    public int LineNumber { get; set; }
    public decimal Quantity { get; set; }
    public string PackageCode { get; set; } = string.Empty;
}
