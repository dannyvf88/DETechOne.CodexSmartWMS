using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Domain.Entities.Inventory;

public sealed class InventoryReservation : BaseEntity
{
    private InventoryReservation()
    {
    }

    public InventoryReservation(string itemCode, string warehouseCode, string locationCode, string? lotNumber, decimal quantity, string referenceType, string referenceNumber, string createdBy)
    {
        ItemCode = NormalizeRequired(itemCode, nameof(itemCode));
        WarehouseCode = NormalizeRequired(warehouseCode, nameof(warehouseCode));
        LocationCode = NormalizeRequired(locationCode, nameof(locationCode));
        LotNumber = NormalizeOptional(lotNumber);
        Quantity = EnsurePositive(quantity, nameof(quantity));
        ReferenceType = NormalizeRequired(referenceType, nameof(referenceType));
        ReferenceNumber = NormalizeRequired(referenceNumber, nameof(referenceNumber));
        Status = InventoryReservationStatus.Active;
        MarkCreated(createdBy);
    }

    public string ItemCode { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string LocationCode { get; private set; } = string.Empty;
    public string? LotNumber { get; private set; }
    public decimal Quantity { get; private set; }
    public string ReferenceType { get; private set; } = string.Empty;
    public string ReferenceNumber { get; private set; } = string.Empty;
    public InventoryReservationStatus Status { get; private set; }

    public void Release(string? user)
    {
        if (Status != InventoryReservationStatus.Active)
        {
            throw new InvalidOperationException("Only active reservations can be released.");
        }

        Status = InventoryReservationStatus.Released;
        MarkUpdated(user);
    }

    public void Consume(string? user)
    {
        if (Status != InventoryReservationStatus.Active)
        {
            throw new InvalidOperationException("Only active reservations can be consumed.");
        }

        Status = InventoryReservationStatus.Consumed;
        MarkUpdated(user);
    }

    public bool MatchesBalance(InventoryBalance balance)
    {
        ArgumentNullException.ThrowIfNull(balance);
        return balance.Matches(ItemCode, WarehouseCode, LocationCode, LotNumber);
    }

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
