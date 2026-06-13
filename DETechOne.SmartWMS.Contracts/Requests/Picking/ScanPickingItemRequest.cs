namespace DETechOne.SmartWMS.Contracts.Requests.Picking;

public sealed class ScanPickingItemRequest
{
    public Guid PickingId { get; set; }
    public int LineNumber { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Barcode { get; set; }
    public string? LotNumber { get; set; }
}
