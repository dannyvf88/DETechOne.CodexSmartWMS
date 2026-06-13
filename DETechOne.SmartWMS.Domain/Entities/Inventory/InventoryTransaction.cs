using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Domain.Entities.Inventory;

public sealed class InventoryTransaction : BaseEntity
{
    private InventoryTransaction()
    {
    }

    public InventoryTransaction(string itemCode, string warehouseCode, string locationCode, string? lotNumber, decimal quantity, InventoryTransactionType transactionType, string reasonCode, string? referenceType, string? referenceNumber, string createdBy)
    {
        ItemCode = NormalizeRequired(itemCode, nameof(itemCode));
        WarehouseCode = NormalizeRequired(warehouseCode, nameof(warehouseCode));
        LocationCode = NormalizeRequired(locationCode, nameof(locationCode));
        LotNumber = NormalizeOptional(lotNumber);
        Quantity = EnsurePositive(quantity, nameof(quantity));
        TransactionType = transactionType;
        ReasonCode = NormalizeRequired(reasonCode, nameof(reasonCode));
        ReferenceType = NormalizeOptional(referenceType);
        ReferenceNumber = NormalizeOptional(referenceNumber);
        MarkCreated(createdBy);
    }

    public string ItemCode { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string LocationCode { get; private set; } = string.Empty;
    public string? LotNumber { get; private set; }
    public decimal Quantity { get; private set; }
    public InventoryTransactionType TransactionType { get; private set; }
    public string ReasonCode { get; private set; } = string.Empty;
    public string? ReferenceType { get; private set; }
    public string? ReferenceNumber { get; private set; }

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
