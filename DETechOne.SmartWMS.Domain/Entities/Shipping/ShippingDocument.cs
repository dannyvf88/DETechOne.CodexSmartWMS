using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Domain.Entities.Shipping;

public sealed class ShippingDocument : BaseEntity
{
    private readonly List<ShippingLine> _lines = new();

    private ShippingDocument()
    {
    }

    public ShippingDocument(
        string shippingNumber,
        Guid packingId,
        string packingNumber,
        string warehouseCode,
        string customerCode,
        string customerName,
        string requestedBy)
    {
        if (packingId == Guid.Empty)
        {
            throw new ArgumentException("PackingId is required.", nameof(packingId));
        }

        ShippingNumber = NormalizeRequired(shippingNumber, nameof(shippingNumber));
        PackingId = packingId;
        PackingNumber = NormalizeRequired(packingNumber, nameof(packingNumber));
        WarehouseCode = NormalizeRequired(warehouseCode, nameof(warehouseCode));
        CustomerCode = NormalizeRequired(customerCode, nameof(customerCode));
        CustomerName = NormalizeRequired(customerName, nameof(customerName));
        RequestedBy = NormalizeRequired(requestedBy, nameof(requestedBy));
        Status = ShippingStatus.Open;
    }

    public string ShippingNumber { get; private set; } = string.Empty;
    public Guid PackingId { get; private set; }
    public string PackingNumber { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string CustomerCode { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;
    public string RequestedBy { get; private set; } = string.Empty;
    public ShippingStatus Status { get; private set; }
    public DateTime? ConfirmedAtUtc { get; private set; }
    public string? ConfirmedBy { get; private set; }
    public int? DeliveryDocEntry { get; private set; }
    public int? DeliveryDocNum { get; private set; }
    public DateTime? DeliveryCreatedAtUtc { get; private set; }
    public string? DeliveryCreatedBy { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public string? CancelledBy { get; private set; }
    public string? CancelReason { get; private set; }
    public IReadOnlyCollection<ShippingLine> Lines => _lines.AsReadOnly();
    public decimal PackedQuantity => _lines.Sum(line => line.PackedQuantity);

    public void AddLine(ShippingLine line)
    {
        ArgumentNullException.ThrowIfNull(line);

        if (Status != ShippingStatus.Open)
        {
            throw new InvalidOperationException("Only open shipping documents can be modified.");
        }

        if (_lines.Any(existing => existing.LineNumber == line.LineNumber))
        {
            throw new InvalidOperationException($"Shipping line {line.LineNumber} already exists.");
        }

        _lines.Add(line);
    }

    public void Confirm(string userName)
    {
        if (Status == ShippingStatus.Cancelled)
        {
            throw new InvalidOperationException("Cancelled shipping documents cannot be confirmed.");
        }

        if (Status == ShippingStatus.DeliveryCreated)
        {
            throw new InvalidOperationException("Shipping document already has a delivery created.");
        }

        if (_lines.Count == 0)
        {
            throw new InvalidOperationException("Shipping document must have at least one line before confirmation.");
        }

        foreach (ShippingLine line in _lines.Where(line => line.Status == ShippingLineStatus.Pending))
        {
            line.Confirm(userName);
        }

        Status = ShippingStatus.Confirmed;
        ConfirmedAtUtc = DateTime.UtcNow;
        ConfirmedBy = NormalizeRequired(userName, nameof(userName));
        MarkUpdated(userName);
    }

    public void MarkDeliveryCreated(int docEntry, int docNum, string userName)
    {
        if (Status != ShippingStatus.Confirmed)
        {
            throw new InvalidOperationException("Only confirmed shipping documents can create SAP deliveries.");
        }

        if (docEntry <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(docEntry), "DocEntry must be greater than zero.");
        }

        if (docNum <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(docNum), "DocNum must be greater than zero.");
        }

        Status = ShippingStatus.DeliveryCreated;
        DeliveryDocEntry = docEntry;
        DeliveryDocNum = docNum;
        DeliveryCreatedAtUtc = DateTime.UtcNow;
        DeliveryCreatedBy = NormalizeRequired(userName, nameof(userName));
        MarkUpdated(userName);
    }

    public void Cancel(string userName, string reason)
    {
        if (Status == ShippingStatus.DeliveryCreated)
        {
            throw new InvalidOperationException("Shipping documents with SAP delivery cannot be cancelled from WMS.");
        }

        if (Status == ShippingStatus.Cancelled)
        {
            throw new InvalidOperationException("Shipping document is already cancelled.");
        }

        foreach (ShippingLine line in _lines.Where(line => line.Status == ShippingLineStatus.Pending))
        {
            line.Cancel(userName);
        }

        Status = ShippingStatus.Cancelled;
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
