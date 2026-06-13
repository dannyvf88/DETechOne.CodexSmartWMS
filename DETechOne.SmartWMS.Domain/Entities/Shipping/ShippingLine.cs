using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Domain.Entities.Shipping;

public sealed class ShippingLine
{
    private ShippingLine()
    {
    }

    public ShippingLine(
        int lineNumber,
        string itemCode,
        string warehouseCode,
        string? locationCode,
        decimal packedQuantity,
        string? lotNumber,
        string? uomCode)
    {
        if (lineNumber < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lineNumber), "LineNumber cannot be negative.");
        }

        if (packedQuantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(packedQuantity), "PackedQuantity must be greater than zero.");
        }

        LineNumber = lineNumber;
        ItemCode = NormalizeRequired(itemCode, nameof(itemCode));
        WarehouseCode = NormalizeRequired(warehouseCode, nameof(warehouseCode));
        LocationCode = NormalizeOptional(locationCode);
        PackedQuantity = packedQuantity;
        LotNumber = NormalizeOptional(lotNumber);
        UomCode = NormalizeOptional(uomCode);
        Status = ShippingLineStatus.Pending;
    }

    public int LineNumber { get; private set; }
    public string ItemCode { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string? LocationCode { get; private set; }
    public decimal PackedQuantity { get; private set; }
    public string? LotNumber { get; private set; }
    public string? UomCode { get; private set; }
    public ShippingLineStatus Status { get; private set; }
    public DateTime? ConfirmedAtUtc { get; private set; }
    public string? ConfirmedBy { get; private set; }

    public void Confirm(string userName)
    {
        if (Status == ShippingLineStatus.Cancelled)
        {
            throw new InvalidOperationException("Cancelled shipping lines cannot be confirmed.");
        }

        Status = ShippingLineStatus.Confirmed;
        ConfirmedAtUtc = DateTime.UtcNow;
        ConfirmedBy = NormalizeRequired(userName, nameof(userName));
    }

    public void Cancel(string userName)
    {
        if (Status == ShippingLineStatus.Confirmed)
        {
            throw new InvalidOperationException("Confirmed shipping lines cannot be cancelled.");
        }

        Status = ShippingLineStatus.Cancelled;
    }

    private static string NormalizeRequired(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim().ToUpperInvariant();
    }

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
}
