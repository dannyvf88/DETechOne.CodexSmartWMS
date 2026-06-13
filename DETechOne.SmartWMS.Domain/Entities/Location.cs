using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Domain.Entities;

public sealed class Location : BaseEntity
{
    private Location()
    {
        WarehouseCode = string.Empty;
        Code = string.Empty;
    }

    public Location(string warehouseCode, string code)
    {
        WarehouseCode = string.IsNullOrWhiteSpace(warehouseCode) ? throw new ArgumentException("Warehouse code is required.", nameof(warehouseCode)) : warehouseCode.Trim();
        Code = string.IsNullOrWhiteSpace(code) ? throw new ArgumentException("Location code is required.", nameof(code)) : code.Trim();
    }

    public string WarehouseCode { get; private set; }
    public string Code { get; private set; }
}
