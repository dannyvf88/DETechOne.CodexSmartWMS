using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Domain.Entities.Packing;

public sealed class PackingDocument : BaseEntity
{
    private readonly List<PackingLine> _lines = new();

    private PackingDocument()
    {
    }

    public PackingDocument(
        string packingNumber,
        Guid pickingId,
        string pickingNumber,
        string warehouseCode,
        string requestedBy)
    {
        if (pickingId == Guid.Empty)
        {
            throw new ArgumentException("PickingId is required.", nameof(pickingId));
        }

        PackingNumber = NormalizeRequired(packingNumber, nameof(packingNumber));
        PickingId = pickingId;
        PickingNumber = NormalizeRequired(pickingNumber, nameof(pickingNumber));
        WarehouseCode = NormalizeRequired(warehouseCode, nameof(warehouseCode));
        RequestedBy = NormalizeRequired(requestedBy, nameof(requestedBy));
        Status = PackingStatus.Open;
    }

    public string PackingNumber { get; private set; } = string.Empty;
    public Guid PickingId { get; private set; }
    public string PickingNumber { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string RequestedBy { get; private set; } = string.Empty;
    public PackingStatus Status { get; private set; }
    public DateTime? StartedAtUtc { get; private set; }
    public string? StartedBy { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public string? CompletedBy { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public string? CancelledBy { get; private set; }
    public string? CancelReason { get; private set; }
    public IReadOnlyCollection<PackingLine> Lines => _lines.AsReadOnly();

    public decimal PickedQuantity => _lines.Sum(line => line.PickedQuantity);
    public decimal PackedQuantity => _lines.Sum(line => line.PackedQuantity);
    public decimal PendingQuantity => PickedQuantity - PackedQuantity;

    public void AddLine(PackingLine line)
    {
        ArgumentNullException.ThrowIfNull(line);

        if (Status != PackingStatus.Open)
        {
            throw new InvalidOperationException("Only open packing documents can be modified.");
        }

        if (_lines.Any(existing => existing.LineNumber == line.LineNumber))
        {
            throw new InvalidOperationException($"Packing line {line.LineNumber} already exists.");
        }

        _lines.Add(line);
    }

    public void PackItem(int lineNumber, decimal quantity, string packageCode, string userName)
    {
        if (Status == PackingStatus.Completed || Status == PackingStatus.Cancelled)
        {
            throw new InvalidOperationException("Completed or cancelled packing documents cannot be packed.");
        }

        PackingLine? line = _lines.FirstOrDefault(item => item.LineNumber == lineNumber);
        if (line is null)
        {
            throw new InvalidOperationException($"Packing line {lineNumber} was not found.");
        }

        if (Status == PackingStatus.Open)
        {
            Status = PackingStatus.InProgress;
            StartedAtUtc = DateTime.UtcNow;
            StartedBy = NormalizeRequired(userName, nameof(userName));
        }

        line.Pack(quantity, packageCode, userName);
        MarkUpdated(userName);

        if (_lines.Count > 0 && _lines.All(item => item.Status == PackingLineStatus.Completed))
        {
            Complete(userName);
        }
    }

    public void Complete(string userName)
    {
        if (Status == PackingStatus.Cancelled)
        {
            throw new InvalidOperationException("Cancelled packing documents cannot be completed.");
        }

        if (_lines.Count == 0)
        {
            throw new InvalidOperationException("Packing document must have at least one line before completion.");
        }

        if (_lines.Any(line => line.Status != PackingLineStatus.Completed))
        {
            throw new InvalidOperationException("All packing lines must be completed before closing the packing document.");
        }

        Status = PackingStatus.Completed;
        CompletedAtUtc = DateTime.UtcNow;
        CompletedBy = NormalizeRequired(userName, nameof(userName));
        MarkUpdated(userName);
    }

    public void Cancel(string userName, string reason)
    {
        if (Status == PackingStatus.Completed)
        {
            throw new InvalidOperationException("Completed packing documents cannot be cancelled.");
        }

        if (Status == PackingStatus.Cancelled)
        {
            throw new InvalidOperationException("Packing document is already cancelled.");
        }

        foreach (PackingLine line in _lines.Where(line => line.Status != PackingLineStatus.Completed))
        {
            line.Cancel(userName);
        }

        Status = PackingStatus.Cancelled;
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
