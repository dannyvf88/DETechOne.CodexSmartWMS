namespace DETechOne.SmartWMS.Contracts.Dtos.Inventory;

public sealed class InventoryTransactionDto
{
    public Guid Id { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public string LocationCode { get; init; } = string.Empty;
    public string? LotNumber { get; init; }
    public decimal Quantity { get; init; }
    public string TransactionType { get; init; } = string.Empty;
    public string ReasonCode { get; init; } = string.Empty;
    public string? ReferenceType { get; init; }
    public string? ReferenceNumber { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
