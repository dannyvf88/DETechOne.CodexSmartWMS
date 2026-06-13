using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Domain.Entities;

public sealed class Warehouse : BaseEntity
{
    private Warehouse()
    {
        Code = string.Empty;
        Name = string.Empty;
    }

    public Warehouse(string code, string name)
    {
        Code = string.IsNullOrWhiteSpace(code) ? throw new ArgumentException("Warehouse code is required.", nameof(code)) : code.Trim();
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Warehouse name is required.", nameof(name)) : name.Trim();
    }

    public string Code { get; private set; }
    public string Name { get; private set; }
}
