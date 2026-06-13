using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Domain.Entities.Picking;

public sealed class PickingDocument : BaseEntity
{
    private readonly List<PickingLine> _lines = new();

    private PickingDocument()
    {
    }

    public PickingDocument(
        string pickingNumber,
        string sourceDocumentType,
        string sourceDocumentNumber,
        string warehouseCode,
        string requestedBy)
    {
        PickingNumber = NormalizeRequired(pickingNumber, nameof(pickingNumber));
        SourceDocumentType = NormalizeRequired(sourceDocumentType, nameof(sourceDocumentType));
        SourceDocumentNumber = NormalizeRequired(sourceDocumentNumber, nameof(sourceDocumentNumber));
        WarehouseCode = NormalizeRequired(warehouseCode, nameof(warehouseCode));
        RequestedBy = NormalizeRequired(requestedBy, nameof(requestedBy));
        Status = PickingStatus.Open;
    }

    public string PickingNumber { get; private set; } = string.Empty;
    public string SourceDocumentType { get; private set; } = string.Empty;
    public string SourceDocumentNumber { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string RequestedBy { get; private set; } = string.Empty;
    public PickingStatus Status { get; private set; }
    public DateTime? StartedAtUtc { get; private set; }
    public string? StartedBy { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public string? CompletedBy { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public string? CancelledBy { get; private set; }
    public string? CancelReason { get; private set; }
    public IReadOnlyCollection<PickingLine> Lines => _lines.AsReadOnly();

    public decimal RequiredQuantity => _lines.Sum(line => line.RequiredQuantity);
    public decimal PickedQuantity => _lines.Sum(line => line.PickedQuantity);
    public decimal PendingQuantity => RequiredQuantity - PickedQuantity;

    public void AddLine(PickingLine line)
    {
        ArgumentNullException.ThrowIfNull(line);

        if (Status != PickingStatus.Open)
        {
            throw new InvalidOperationException("Only open picking documents can be modified.");
        }

        if (_lines.Any(existing => existing.LineNumber == line.LineNumber))
        {
            throw new InvalidOperationException($"Picking line {line.LineNumber} already exists.");
        }

        _lines.Add(line);
    }

    public void ScanItem(int lineNumber, decimal quantity, string userName)
    {
        if (Status == PickingStatus.Completed || Status == PickingStatus.Cancelled)
        {
            throw new InvalidOperationException("Completed or cancelled picking documents cannot be scanned.");
        }

        PickingLine? line = _lines.FirstOrDefault(item => item.LineNumber == lineNumber);
        if (line is null)
        {
            throw new InvalidOperationException($"Picking line {lineNumber} was not found.");
        }

        if (Status == PickingStatus.Open)
        {
            Status = PickingStatus.InProgress;
            StartedAtUtc = DateTime.UtcNow;
            StartedBy = NormalizeRequired(userName, nameof(userName));
        }

        line.Scan(quantity, userName);
        MarkUpdated(userName);

        if (_lines.Count > 0 && _lines.All(item => item.Status == PickingLineStatus.Completed))
        {
            Complete(userName);
        }
    }

    public void Complete(string userName)
    {
        if (Status == PickingStatus.Cancelled)
        {
            throw new InvalidOperationException("Cancelled picking documents cannot be completed.");
        }

        if (_lines.Count == 0)
        {
            throw new InvalidOperationException("Picking document must have at least one line before completion.");
        }

        if (_lines.Any(line => line.Status != PickingLineStatus.Completed))
        {
            throw new InvalidOperationException("All picking lines must be completed before closing the picking document.");
        }

        Status = PickingStatus.Completed;
        CompletedAtUtc = DateTime.UtcNow;
        CompletedBy = NormalizeRequired(userName, nameof(userName));
        MarkUpdated(userName);
    }

    public void Cancel(string userName, string reason)
    {
        if (Status == PickingStatus.Completed)
        {
            throw new InvalidOperationException("Completed picking documents cannot be cancelled.");
        }

        if (Status == PickingStatus.Cancelled)
        {
            throw new InvalidOperationException("Picking document is already cancelled.");
        }

        foreach (PickingLine line in _lines.Where(line => line.Status != PickingLineStatus.Completed))
        {
            line.Cancel(userName);
        }

        Status = PickingStatus.Cancelled;
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
}
