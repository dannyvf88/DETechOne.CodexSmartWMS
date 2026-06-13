using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Domain.Entities.Picking;

public sealed class PickingLine : BaseEntity
{
    private PickingLine()
    {
    }

    public PickingLine(
        int lineNumber,
        string itemCode,
        string warehouseCode,
        string? locationCode,
        decimal requiredQuantity,
        string? lotNumber,
        string? uomCode)
    {
        if (lineNumber < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lineNumber), "Line number cannot be negative.");
        }

        if (requiredQuantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(requiredQuantity), "Required quantity must be greater than zero.");
        }

        LineNumber = lineNumber;
        ItemCode = NormalizeRequired(itemCode, nameof(itemCode));
        WarehouseCode = NormalizeRequired(warehouseCode, nameof(warehouseCode));
        LocationCode = NormalizeOptional(locationCode);
        RequiredQuantity = requiredQuantity;
        PickedQuantity = 0;
        LotNumber = NormalizeOptional(lotNumber);
        UomCode = NormalizeOptional(uomCode);
        Status = PickingLineStatus.Pending;
    }

    public int LineNumber { get; private set; }
    public string ItemCode { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string? LocationCode { get; private set; }
    public decimal RequiredQuantity { get; private set; }
    public decimal PickedQuantity { get; private set; }
    public decimal PendingQuantity => RequiredQuantity - PickedQuantity;
    public string? LotNumber { get; private set; }
    public string? UomCode { get; private set; }
    public PickingLineStatus Status { get; private set; }

    public void Scan(decimal quantity, string userName)
    {
        if (Status == PickingLineStatus.Cancelled)
        {
            throw new InvalidOperationException("Cancelled picking lines cannot be scanned.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Scan quantity must be greater than zero.");
        }

        if (PickedQuantity + quantity > RequiredQuantity)
        {
            throw new InvalidOperationException("Scanned quantity cannot exceed required quantity.");
        }

        PickedQuantity += quantity;
        Status = PickedQuantity == RequiredQuantity ? PickingLineStatus.Completed : PickingLineStatus.Partial;
        MarkUpdated(userName);
    }

    public void Cancel(string userName)
    {
        Status = PickingLineStatus.Cancelled;
        MarkUpdated(userName);
    }

    private static string NormalizeRequired(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim().ToUpperInvariant();
    }

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
}
