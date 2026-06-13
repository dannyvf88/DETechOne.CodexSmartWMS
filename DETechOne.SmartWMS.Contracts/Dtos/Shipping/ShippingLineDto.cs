namespace DETechOne.SmartWMS.Contracts.Dtos.Shipping;

public sealed class ShippingLineDto
{
    public int LineNumber { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public string? LocationCode { get; init; }
    public decimal PackedQuantity { get; init; }
    public string? LotNumber { get; init; }
    public string? UomCode { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime? ConfirmedAtUtc { get; init; }
    public string? ConfirmedBy { get; init; }
}
