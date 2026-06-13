namespace DETechOne.SmartWMS.Contracts.Requests.Inventory;

public sealed class GetInventoryAvailabilityRequest
{
    public string ItemCode { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public string? LocationCode { get; init; }
    public string? LotNumber { get; init; }
    public decimal RequestedQuantity { get; init; }
}
