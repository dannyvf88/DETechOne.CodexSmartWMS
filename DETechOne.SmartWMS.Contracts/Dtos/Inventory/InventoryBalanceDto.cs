namespace DETechOne.SmartWMS.Contracts.Dtos.Inventory;

public sealed class InventoryBalanceDto
{
    public Guid Id { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public string LocationCode { get; init; } = string.Empty;
    public string? LotNumber { get; init; }
    public decimal OnHandQuantity { get; init; }
    public decimal ReservedQuantity { get; init; }
    public decimal AvailableQuantity { get; init; }
}
