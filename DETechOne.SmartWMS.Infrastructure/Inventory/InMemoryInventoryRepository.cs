using DETechOne.SmartWMS.Application.Inventory;
using DETechOne.SmartWMS.Domain.Entities.Inventory;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Infrastructure.Inventory;

public sealed class InMemoryInventoryRepository : IInventoryRepository
{
    private readonly List<InventoryBalance> _balances = new();
    private readonly List<InventoryReservation> _reservations = new();
    private readonly List<InventoryTransaction> _transactions = new();
    private readonly object _syncRoot = new();

    public Task<IReadOnlyList<InventoryBalance>> GetBalancesAsync(string itemCode, string warehouseCode, string? locationCode = null, string? lotNumber = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_syncRoot)
        {
            IReadOnlyList<InventoryBalance> result = _balances
                .Where(balance => Matches(balance, itemCode, warehouseCode, locationCode, lotNumber))
                .ToArray();

            return Task.FromResult(result);
        }
    }

    public Task<InventoryBalance?> GetBalanceAsync(string itemCode, string warehouseCode, string locationCode, string? lotNumber, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_syncRoot)
        {
            InventoryBalance? balance = _balances.FirstOrDefault(current => current.Matches(itemCode, warehouseCode, locationCode, lotNumber));
            return Task.FromResult(balance);
        }
    }

    public Task UpsertBalanceAsync(InventoryBalance balance, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(balance);
        cancellationToken.ThrowIfCancellationRequested();

        lock (_syncRoot)
        {
            InventoryBalance? existing = _balances.FirstOrDefault(current => current.Id == balance.Id || current.Matches(balance.ItemCode, balance.WarehouseCode, balance.LocationCode, balance.LotNumber));
            if (existing is null)
            {
                _balances.Add(balance);
            }
        }

        return Task.CompletedTask;
    }

    public Task AddTransactionAsync(InventoryTransaction transaction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        cancellationToken.ThrowIfCancellationRequested();

        lock (_syncRoot)
        {
            _transactions.Add(transaction);
        }

        return Task.CompletedTask;
    }

    public Task AddReservationAsync(InventoryReservation reservation, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reservation);
        cancellationToken.ThrowIfCancellationRequested();

        lock (_syncRoot)
        {
            _reservations.Add(reservation);
        }

        return Task.CompletedTask;
    }

    public Task<InventoryReservation?> GetReservationAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_syncRoot)
        {
            InventoryReservation? reservation = _reservations.FirstOrDefault(current => current.Id == reservationId);
            return Task.FromResult(reservation);
        }
    }

    public Task UpdateReservationAsync(InventoryReservation reservation, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reservation);
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<InventoryReservation>> GetActiveReservationsAsync(string itemCode, string warehouseCode, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_syncRoot)
        {
            IReadOnlyList<InventoryReservation> result = _reservations
                .Where(reservation => reservation.Status == InventoryReservationStatus.Active
                    && reservation.ItemCode.Equals(Normalize(itemCode), StringComparison.OrdinalIgnoreCase)
                    && reservation.WarehouseCode.Equals(Normalize(warehouseCode), StringComparison.OrdinalIgnoreCase))
                .ToArray();

            return Task.FromResult(result);
        }
    }

    private static bool Matches(InventoryBalance balance, string itemCode, string warehouseCode, string? locationCode, string? lotNumber)
    {
        if (!balance.ItemCode.Equals(Normalize(itemCode), StringComparison.OrdinalIgnoreCase)) return false;
        if (!balance.WarehouseCode.Equals(Normalize(warehouseCode), StringComparison.OrdinalIgnoreCase)) return false;
        if (!string.IsNullOrWhiteSpace(locationCode) && !balance.LocationCode.Equals(Normalize(locationCode), StringComparison.OrdinalIgnoreCase)) return false;
        if (!string.IsNullOrWhiteSpace(lotNumber) && !string.Equals(balance.LotNumber, Normalize(lotNumber), StringComparison.OrdinalIgnoreCase)) return false;
        return true;
    }

    private static string Normalize(string value) => value.Trim().ToUpperInvariant();
}
