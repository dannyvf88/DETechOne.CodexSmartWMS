namespace DETechOne.SmartWMS.Contracts.Dtos.SAP;

public sealed class SapWarehouseDto
{
    public string WarehouseCode { get; init; } = string.Empty;
    public string WarehouseName { get; init; } = string.Empty;
    public bool Inactive { get; init; }
}
