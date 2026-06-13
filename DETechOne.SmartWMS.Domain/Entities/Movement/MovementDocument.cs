using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Domain.Entities.Movement;

public sealed class MovementDocument : BaseEntity
{
    private readonly List<MovementLine> _lines = new();

    private MovementDocument()
    {
    }

    public MovementDocument(
        string movementNumber,
        MovementType movementType,
        string referenceType,
        string? referenceNumber,
        string requestedBy)
    {
        MovementNumber = NormalizeRequired(movementNumber, nameof(movementNumber));
        MovementType = movementType;
        ReferenceType = NormalizeRequired(referenceType, nameof(referenceType));
        ReferenceNumber = NormalizeOptional(referenceNumber);
        RequestedBy = NormalizeRequired(requestedBy, nameof(requestedBy));
        Status = MovementStatus.Open;
    }

    public string MovementNumber { get; private set; } = string.Empty;
    public MovementType MovementType { get; private set; }
    public MovementStatus Status { get; private set; }
    public string ReferenceType { get; private set; } = string.Empty;
    public string? ReferenceNumber { get; private set; }
    public string RequestedBy { get; private set; } = string.Empty;
    public DateTime? ConfirmedAtUtc { get; private set; }
    public string? ConfirmedBy { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public string? CancelledBy { get; private set; }
    public string? CancelReason { get; private set; }
    public IReadOnlyCollection<MovementLine> Lines => _lines.AsReadOnly();

    public void AddLine(MovementLine line)
    {
        ArgumentNullException.ThrowIfNull(line);

        if (Status != MovementStatus.Open)
        {
            throw new InvalidOperationException("Only open movement documents can be modified.");
        }

        if (_lines.Any(existing => existing.LineNumber == line.LineNumber))
        {
            throw new InvalidOperationException($"Movement line {line.LineNumber} already exists.");
        }

        _lines.Add(line);
    }

    public void Confirm(string userName)
    {
        if (Status != MovementStatus.Open)
        {
            throw new InvalidOperationException("Only open movement documents can be confirmed.");
        }

        if (_lines.Count == 0)
        {
            throw new InvalidOperationException("Movement document must have at least one line before confirmation.");
        }

        Status = MovementStatus.Confirmed;
        ConfirmedAtUtc = DateTime.UtcNow;
        ConfirmedBy = NormalizeRequired(userName, nameof(userName));
        MarkUpdated(userName);
    }

    public void Cancel(string userName, string reason)
    {
        if (Status == MovementStatus.Confirmed)
        {
            throw new InvalidOperationException("Confirmed movement documents cannot be cancelled.");
        }

        if (Status == MovementStatus.Cancelled)
        {
            throw new InvalidOperationException("Movement document is already cancelled.");
        }

        Status = MovementStatus.Cancelled;
        CancelledAtUtc = DateTime.UtcNow;
        CancelledBy = NormalizeRequired(userName, nameof(userName));
        CancelReason = NormalizeRequired(reason, nameof(reason));
        MarkUpdated(userName);
    }

    private static string NormalizeRequired(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim().ToUpperInvariant();
    }

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
}
