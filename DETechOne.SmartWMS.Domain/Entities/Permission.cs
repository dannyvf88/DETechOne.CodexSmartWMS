using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Domain.Entities;

public sealed class Permission : BaseEntity
{
    private Permission()
    {
        Code = string.Empty;
        Name = string.Empty;
    }

    public Permission(string code, string name)
    {
        Code = string.IsNullOrWhiteSpace(code) ? throw new ArgumentException("Permission code is required.", nameof(code)) : code.Trim();
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Permission name is required.", nameof(name)) : name.Trim();
    }

    public string Code { get; private set; }
    public string Name { get; private set; }
}
