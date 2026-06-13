namespace DETechOne.SmartWMS.Contracts.Requests.Inventory;

public sealed class ReserveInventoryRequest
{
    public string ItemCode { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public string LocationCode { get; init; } = string.Empty;
    public string? LotNumber { get; init; }
    public decimal Quantity { get; init; }
    public string ReferenceType { get; init; } = string.Empty;
    public string ReferenceNumber { get; init; } = string.Empty;
}
