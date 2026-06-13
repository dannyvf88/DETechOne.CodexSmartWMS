namespace DETechOne.SmartWMS.Contracts.Dtos.SAP;

public sealed class SapItemDto
{
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public bool InventoryItem { get; init; }
    public bool SalesItem { get; init; }
    public bool PurchaseItem { get; init; }
    public string InventoryUom { get; init; } = string.Empty;
}
