namespace DETechOne.SmartWMS.Contracts.Requests.Shipping;

public sealed class CreateShippingLineRequest
{
    public int LineNumber { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public string? LocationCode { get; init; }
    public decimal PackedQuantity { get; init; }
    public string? LotNumber { get; init; }
    public string? UomCode { get; init; }
}
