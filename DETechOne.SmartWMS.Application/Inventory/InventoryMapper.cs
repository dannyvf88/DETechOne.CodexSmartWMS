using DETechOne.SmartWMS.Contracts.Dtos.Inventory;
using DETechOne.SmartWMS.Domain.Entities.Inventory;

namespace DETechOne.SmartWMS.Application.Inventory;

internal static class InventoryMapper
{
    public static InventoryBalanceDto ToDto(InventoryBalance balance)
    {
        return new InventoryBalanceDto
        {
            Id = balance.Id,
            ItemCode = balance.ItemCode,
            WarehouseCode = balance.WarehouseCode,
            LocationCode = balance.LocationCode,
            LotNumber = balance.LotNumber,
            OnHandQuantity = balance.OnHandQuantity,
            ReservedQuantity = balance.ReservedQuantity,
            AvailableQuantity = balance.AvailableQuantity
        };
    }

    public static InventoryReservationDto ToDto(InventoryReservation reservation)
    {
        return new InventoryReservationDto
        {
            Id = reservation.Id,
            ItemCode = reservation.ItemCode,
            WarehouseCode = reservation.WarehouseCode,
            LocationCode = reservation.LocationCode,
            LotNumber = reservation.LotNumber,
            Quantity = reservation.Quantity,
            ReferenceType = reservation.ReferenceType,
            ReferenceNumber = reservation.ReferenceNumber,
            Status = reservation.Status.ToString()
        };
    }

    public static InventoryTransactionDto ToDto(InventoryTransaction transaction)
    {
        return new InventoryTransactionDto
        {
            Id = transaction.Id,
            ItemCode = transaction.ItemCode,
            WarehouseCode = transaction.WarehouseCode,
            LocationCode = transaction.LocationCode,
            LotNumber = transaction.LotNumber,
            Quantity = transaction.Quantity,
            TransactionType = transaction.TransactionType.ToString(),
            ReasonCode = transaction.ReasonCode,
            ReferenceType = transaction.ReferenceType,
            ReferenceNumber = transaction.ReferenceNumber,
            CreatedAtUtc = transaction.CreatedAtUtc
        };
    }
}
