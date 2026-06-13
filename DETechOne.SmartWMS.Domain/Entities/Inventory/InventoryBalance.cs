using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Domain.Entities.Inventory;

public sealed class InventoryBalance : BaseEntity
{
    private InventoryBalance()
    {
    }

    public InventoryBalance(string itemCode, string warehouseCode, string locationCode, string? lotNumber, decimal onHandQuantity, decimal reservedQuantity = 0)
    {
        ItemCode = NormalizeRequired(itemCode, nameof(itemCode));
        WarehouseCode = NormalizeRequired(warehouseCode, nameof(warehouseCode));
        LocationCode = NormalizeRequired(locationCode, nameof(locationCode));
        LotNumber = NormalizeOptional(lotNumber);
        OnHandQuantity = EnsureNonNegative(onHandQuantity, nameof(onHandQuantity));
        ReservedQuantity = EnsureNonNegative(reservedQuantity, nameof(reservedQuantity));
    }

    public string ItemCode { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string LocationCode { get; private set; } = string.Empty;
    public string? LotNumber { get; private set; }
    public decimal OnHandQuantity { get; private set; }
    public decimal ReservedQuantity { get; private set; }
    public decimal AvailableQuantity => OnHandQuantity - ReservedQuantity;

    public void Increase(decimal quantity)
    {
        OnHandQuantity += EnsurePositive(quantity, nameof(quantity));
    }

    public void Decrease(decimal quantity)
    {
        decimal normalizedQuantity = EnsurePositive(quantity, nameof(quantity));

        if (OnHandQuantity - normalizedQuantity < 0)
        {
            throw new InvalidOperationException("Inventory cannot be decreased below zero.");
        }

        OnHandQuantity -= normalizedQuantity;

        if (ReservedQuantity > OnHandQuantity)
        {
            ReservedQuantity = OnHandQuantity;
        }
    }

    public void Reserve(decimal quantity)
    {
        decimal normalizedQuantity = EnsurePositive(quantity, nameof(quantity));

        if (AvailableQuantity < normalizedQuantity)
        {
            throw new InvalidOperationException("There is not enough available inventory to reserve.");
        }

        ReservedQuantity += normalizedQuantity;
    }

    public void Release(decimal quantity)
    {
        decimal normalizedQuantity = EnsurePositive(quantity, nameof(quantity));

        if (ReservedQuantity - normalizedQuantity < 0)
        {
            throw new InvalidOperationException("Reserved quantity cannot be released below zero.");
        }

        ReservedQuantity -= normalizedQuantity;
    }

    public bool Matches(string itemCode, string warehouseCode, string locationCode, string? lotNumber)
    {
        return ItemCode.Equals(NormalizeRequired(itemCode, nameof(itemCode)), StringComparison.OrdinalIgnoreCase)
            && WarehouseCode.Equals(NormalizeRequired(warehouseCode, nameof(warehouseCode)), StringComparison.OrdinalIgnoreCase)
            && LocationCode.Equals(NormalizeRequired(locationCode, nameof(locationCode)), StringComparison.OrdinalIgnoreCase)
            && string.Equals(LotNumber, NormalizeOptional(lotNumber), StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeRequired(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim().ToUpperInvariant();
    }

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();

    private static decimal EnsureNonNegative(decimal value, string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, "The value cannot be negative.");
        }

        return value;
    }

    private static decimal EnsurePositive(decimal value, string parameterName)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, "The value must be greater than zero.");
        }

        return value;
    }
}
