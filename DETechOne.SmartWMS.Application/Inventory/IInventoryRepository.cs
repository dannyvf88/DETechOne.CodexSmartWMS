using DETechOne.SmartWMS.Domain.Entities.Inventory;

namespace DETechOne.SmartWMS.Application.Inventory;

public interface IInventoryRepository
{
    Task<IReadOnlyList<InventoryBalance>> GetBalancesAsync(string itemCode, string warehouseCode, string? locationCode = null, string? lotNumber = null, CancellationToken cancellationToken = default);
    Task<InventoryBalance?> GetBalanceAsync(string itemCode, string warehouseCode, string locationCode, string? lotNumber, CancellationToken cancellationToken = default);
    Task UpsertBalanceAsync(InventoryBalance balance, CancellationToken cancellationToken = default);
    Task AddTransactionAsync(InventoryTransaction transaction, CancellationToken cancellationToken = default);
    Task AddReservationAsync(InventoryReservation reservation, CancellationToken cancellationToken = default);
    Task<InventoryReservation?> GetReservationAsync(Guid reservationId, CancellationToken cancellationToken = default);
    Task UpdateReservationAsync(InventoryReservation reservation, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InventoryReservation>> GetActiveReservationsAsync(string itemCode, string warehouseCode, CancellationToken cancellationToken = default);
}
