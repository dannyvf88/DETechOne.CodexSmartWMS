namespace DETechOne.SmartWMS.Contracts.Dtos.Inventory;

public sealed class InventoryReservationDto
{
    public Guid Id { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public string LocationCode { get; init; } = string.Empty;
    public string? LotNumber { get; init; }
    public decimal Quantity { get; init; }
    public string ReferenceType { get; init; } = string.Empty;
    public string ReferenceNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}
