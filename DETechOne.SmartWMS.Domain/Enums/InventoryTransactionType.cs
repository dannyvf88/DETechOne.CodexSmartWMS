namespace DETechOne.SmartWMS.Domain.Enums;

public enum InventoryTransactionType
{
    AdjustmentIn = 1,
    AdjustmentOut = 2,
    Receipt = 3,
    Issue = 4,
    TransferIn = 5,
    TransferOut = 6,
    PickingReserve = 7,
    PackingCommit = 8,
    ReservationRelease = 9
}
