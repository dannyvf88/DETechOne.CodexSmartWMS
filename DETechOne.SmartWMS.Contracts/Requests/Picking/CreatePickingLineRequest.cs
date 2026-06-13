namespace DETechOne.SmartWMS.Contracts.Requests.Picking;

public sealed class CreatePickingLineRequest
{
    public int LineNumber { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string WarehouseCode { get; set; } = string.Empty;
    public string? LocationCode { get; set; }
    public decimal RequiredQuantity { get; set; }
    public string? LotNumber { get; set; }
    public string? UomCode { get; set; }
}
