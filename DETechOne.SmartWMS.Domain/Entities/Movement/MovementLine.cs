using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Domain.Entities.Movement;

public sealed class MovementLine : BaseEntity
{
    private MovementLine()
    {
    }

    public MovementLine(
        int lineNumber,
        string itemCode,
        string fromWarehouseCode,
        string fromLocationCode,
        string toWarehouseCode,
        string toLocationCode,
        decimal quantity,
        string? lotNumber = null,
        string? uomCode = null)
    {
        if (lineNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lineNumber), "LineNumber must be greater than zero.");
        }

        LineNumber = lineNumber;
        ItemCode = NormalizeRequired(itemCode, nameof(itemCode));
        FromWarehouseCode = NormalizeRequired(fromWarehouseCode, nameof(fromWarehouseCode));
        FromLocationCode = NormalizeRequired(fromLocationCode, nameof(fromLocationCode));
        ToWarehouseCode = NormalizeRequired(toWarehouseCode, nameof(toWarehouseCode));
        ToLocationCode = NormalizeRequired(toLocationCode, nameof(toLocationCode));
        Quantity = EnsurePositive(quantity, nameof(quantity));
        LotNumber = NormalizeOptional(lotNumber);
        UomCode = NormalizeOptional(uomCode);
    }

    public int LineNumber { get; private set; }
    public string ItemCode { get; private set; } = string.Empty;
    public string FromWarehouseCode { get; private set; } = string.Empty;
    public string FromLocationCode { get; private set; } = string.Empty;
    public string ToWarehouseCode { get; private set; } = string.Empty;
    public string ToLocationCode { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public string? LotNumber { get; private set; }
    public string? UomCode { get; private set; }

    private static string NormalizeRequired(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim().ToUpperInvariant();
    }

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();

    private static decimal EnsurePositive(decimal value, string parameterName)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, "The value must be greater than zero.");
        }

        return value;
    }
}
