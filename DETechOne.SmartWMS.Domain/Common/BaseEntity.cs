namespace DETechOne.SmartWMS.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAtUtc { get; protected set; } = DateTime.UtcNow;
    public string? CreatedBy { get; protected set; }
    public DateTime? UpdatedAtUtc { get; protected set; }
    public string? UpdatedBy { get; protected set; }
    public DateTime? DeletedAtUtc { get; protected set; }
    public string? DeletedBy { get; protected set; }
    public bool IsDeleted => DeletedAtUtc.HasValue;

    public void MarkCreated(string? user)
    {
        CreatedAtUtc = DateTime.UtcNow;
        CreatedBy = user;
    }

    public void MarkUpdated(string? user)
    {
        UpdatedAtUtc = DateTime.UtcNow;
        UpdatedBy = user;
    }

    public void MarkDeleted(string? user)
    {
        DeletedAtUtc = DateTime.UtcNow;
        DeletedBy = user;
    }
}
