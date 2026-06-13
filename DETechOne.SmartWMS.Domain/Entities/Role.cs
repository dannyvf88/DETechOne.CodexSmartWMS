using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Domain.Entities;

public sealed class Role : BaseEntity
{
    private readonly List<RolePermission> _permissions = new();

    private Role()
    {
        Code = string.Empty;
        Name = string.Empty;
    }

    public Role(string code, string name)
    {
        Code = string.IsNullOrWhiteSpace(code) ? throw new ArgumentException("Role code is required.", nameof(code)) : code.Trim();
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Role name is required.", nameof(name)) : name.Trim();
    }

    public string Code { get; private set; }
    public string Name { get; private set; }
    public IReadOnlyCollection<RolePermission> Permissions => _permissions.AsReadOnly();

    public void GrantPermission(Permission permission)
    {
        ArgumentNullException.ThrowIfNull(permission);

        if (_permissions.Any(x => x.PermissionId == permission.Id))
        {
            return;
        }

        _permissions.Add(new RolePermission(Id, permission.Id));
    }
}
