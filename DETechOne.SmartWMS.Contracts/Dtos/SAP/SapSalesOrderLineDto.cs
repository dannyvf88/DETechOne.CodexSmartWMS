namespace DETechOne.SmartWMS.Contracts.Dtos.SAP;

public sealed class SapSalesOrderLineDto
{
    public int LineNum { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string ItemDescription { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public decimal OpenQuantity { get; init; }
    public string UomCode { get; init; } = string.Empty;
}
