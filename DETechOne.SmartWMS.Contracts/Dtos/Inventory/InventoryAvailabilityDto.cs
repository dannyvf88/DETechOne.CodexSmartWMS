namespace DETechOne.SmartWMS.Contracts.Dtos.Inventory;

public sealed class InventoryAvailabilityDto
{
    public string ItemCode { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public string? LocationCode { get; init; }
    public string? LotNumber { get; init; }
    public decimal RequestedQuantity { get; init; }
    public decimal OnHandQuantity { get; init; }
    public decimal ReservedQuantity { get; init; }
    public decimal AvailableQuantity { get; init; }
    public string Status { get; init; } = string.Empty;
    public IReadOnlyList<InventoryBalanceDto> Balances { get; init; } = Array.Empty<InventoryBalanceDto>();
}
