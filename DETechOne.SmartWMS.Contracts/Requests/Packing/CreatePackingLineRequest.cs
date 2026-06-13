namespace DETechOne.SmartWMS.Contracts.Requests.Packing;

public sealed class CreatePackingLineRequest
{
    public int LineNumber { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string WarehouseCode { get; set; } = string.Empty;
    public string? LocationCode { get; set; }
    public decimal PickedQuantity { get; set; }
    public string? LotNumber { get; set; }
    public string? UomCode { get; set; }
}
