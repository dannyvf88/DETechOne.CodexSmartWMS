using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Domain.Entities;

public sealed class User : BaseEntity
{
    private readonly List<UserRole> _roles = new();

    private User()
    {
        UserName = string.Empty;
        DisplayName = string.Empty;
        PasswordHash = string.Empty;
        Email = string.Empty;
    }

    public User(string userName, string displayName, string email, string passwordHash)
    {
        UserName = string.IsNullOrWhiteSpace(userName) ? throw new ArgumentException("User name is required.", nameof(userName)) : userName.Trim();
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? throw new ArgumentException("Display name is required.", nameof(displayName)) : displayName.Trim();
        Email = string.IsNullOrWhiteSpace(email) ? throw new ArgumentException("Email is required.", nameof(email)) : email.Trim();
        PasswordHash = string.IsNullOrWhiteSpace(passwordHash) ? throw new ArgumentException("Password hash is required.", nameof(passwordHash)) : passwordHash;
        Status = UserStatus.Active;
    }

    public string UserName { get; private set; }
    public string DisplayName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserStatus Status { get; private set; }
    public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();

    public bool IsActive => Status == UserStatus.Active && !IsDeleted;

    public void AssignRole(Role role)
    {
        ArgumentNullException.ThrowIfNull(role);

        if (_roles.Any(x => x.RoleId == role.Id))
        {
            return;
        }

        _roles.Add(new UserRole(Id, role.Id));
    }

    public void Lock() => Status = UserStatus.Locked;
    public void Deactivate() => Status = UserStatus.Inactive;
    public void Activate() => Status = UserStatus.Active;
}
