namespace DETechOne.SmartWMS.Contracts.Dtos.Picking;

public sealed class PickingLineDto
{
    public Guid Id { get; set; }
    public int LineNumber { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string WarehouseCode { get; set; } = string.Empty;
    public string? LocationCode { get; set; }
    public decimal RequiredQuantity { get; set; }
    public decimal PickedQuantity { get; set; }
    public decimal PendingQuantity { get; set; }
    public string? LotNumber { get; set; }
    public string? UomCode { get; set; }
    public string Status { get; set; } = string.Empty;
}
