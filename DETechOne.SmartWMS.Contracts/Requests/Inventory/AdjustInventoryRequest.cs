namespace DETechOne.SmartWMS.Contracts.Requests.Inventory;

public sealed class AdjustInventoryRequest
{
    public string ItemCode { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public string LocationCode { get; init; } = string.Empty;
    public string? LotNumber { get; init; }
    public decimal Quantity { get; init; }
    public string AdjustmentType { get; init; } = "IN";
    public string ReasonCode { get; init; } = "MANUAL";
    public string? ReferenceType { get; init; }
    public string? ReferenceNumber { get; init; }
}
