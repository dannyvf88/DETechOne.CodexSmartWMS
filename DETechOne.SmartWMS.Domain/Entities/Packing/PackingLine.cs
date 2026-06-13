using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Domain.Entities.Packing;

public sealed class PackingLine : BaseEntity
{
    private PackingLine()
    {
    }

    public PackingLine(
        int lineNumber,
        string itemCode,
        string warehouseCode,
        string? locationCode,
        decimal pickedQuantity,
        string? lotNumber,
        string? uomCode)
    {
        if (lineNumber < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lineNumber), "Line number cannot be negative.");
        }

        if (pickedQuantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pickedQuantity), "Picked quantity must be greater than zero.");
        }

        LineNumber = lineNumber;
        ItemCode = NormalizeRequired(itemCode, nameof(itemCode));
        WarehouseCode = NormalizeRequired(warehouseCode, nameof(warehouseCode));
        LocationCode = NormalizeOptional(locationCode);
        PickedQuantity = pickedQuantity;
        PackedQuantity = 0;
        LotNumber = NormalizeOptional(lotNumber);
        UomCode = NormalizeOptional(uomCode);
        Status = PackingLineStatus.Pending;
    }

    public int LineNumber { get; private set; }
    public string ItemCode { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string? LocationCode { get; private set; }
    public decimal PickedQuantity { get; private set; }
    public decimal PackedQuantity { get; private set; }
    public decimal PendingQuantity => PickedQuantity - PackedQuantity;
    public string? LotNumber { get; private set; }
    public string? UomCode { get; private set; }
    public string? PackageCode { get; private set; }
    public PackingLineStatus Status { get; private set; }

    public void Pack(decimal quantity, string packageCode, string userName)
    {
        if (Status == PackingLineStatus.Cancelled)
        {
            throw new InvalidOperationException("Cancelled packing lines cannot be packed.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Pack quantity must be greater than zero.");
        }

        if (PackedQuantity + quantity > PickedQuantity)
        {
            throw new InvalidOperationException("Packed quantity cannot exceed picked quantity.");
        }

        PackageCode = NormalizeRequired(packageCode, nameof(packageCode));
        PackedQuantity += quantity;
        Status = PackedQuantity == PickedQuantity ? PackingLineStatus.Completed : PackingLineStatus.Partial;
        MarkUpdated(userName);
    }

    public void Cancel(string userName)
    {
        Status = PackingLineStatus.Cancelled;
        MarkUpdated(userName);
    }

    private static string NormalizeRequired(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim().ToUpperInvariant();
    }

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
}
