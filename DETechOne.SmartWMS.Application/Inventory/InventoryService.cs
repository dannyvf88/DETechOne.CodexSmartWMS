using DETechOne.SmartWMS.Contracts.Dtos.Inventory;
using DETechOne.SmartWMS.Contracts.Requests.Inventory;
using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Entities.Inventory;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Application.Inventory;

public sealed class InventoryService : IInventoryService
{
    private const string DefaultSystemUser = "system";
    private readonly IInventoryRepository _inventoryRepository;

    public InventoryService(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<Result<InventoryAvailabilityDto>> GetAvailabilityAsync(GetInventoryAvailabilityRequest request, CancellationToken cancellationToken = default)
    {
        string validationError = ValidateAvailabilityRequest(request);
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            return Result<InventoryAvailabilityDto>.Fail("INVENTORY_VALIDATION", validationError);
        }

        IReadOnlyList<InventoryBalance> balances = await _inventoryRepository
            .GetBalancesAsync(request.ItemCode, request.WarehouseCode, request.LocationCode, request.LotNumber, cancellationToken)
            .ConfigureAwait(false);

        decimal onHand = balances.Sum(balance => balance.OnHandQuantity);
        decimal reserved = balances.Sum(balance => balance.ReservedQuantity);
        decimal available = balances.Sum(balance => balance.AvailableQuantity);
        StockAvailabilityStatus status = GetStatus(available, request.RequestedQuantity);

        var dto = new InventoryAvailabilityDto
        {
            ItemCode = Normalize(request.ItemCode),
            WarehouseCode = Normalize(request.WarehouseCode),
            LocationCode = NormalizeOptional(request.LocationCode),
            LotNumber = NormalizeOptional(request.LotNumber),
            RequestedQuantity = request.RequestedQuantity,
            OnHandQuantity = onHand,
            ReservedQuantity = reserved,
            AvailableQuantity = available,
            Status = status.ToString(),
            Balances = balances.Select(InventoryMapper.ToDto).ToArray()
        };

        return Result<InventoryAvailabilityDto>.Ok(dto);
    }

    public async Task<Result<InventoryBalanceDto>> AdjustAsync(AdjustInventoryRequest request, string? userName, CancellationToken cancellationToken = default)
    {
        string validationError = ValidateAdjustmentRequest(request);
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            return Result<InventoryBalanceDto>.Fail("INVENTORY_VALIDATION", validationError);
        }

        InventoryBalance balance = await _inventoryRepository
            .GetBalanceAsync(request.ItemCode, request.WarehouseCode, request.LocationCode, request.LotNumber, cancellationToken)
            .ConfigureAwait(false)
            ?? new InventoryBalance(request.ItemCode, request.WarehouseCode, request.LocationCode, request.LotNumber, 0);

        InventoryTransactionType transactionType = IsAdjustmentIn(request.AdjustmentType)
            ? InventoryTransactionType.AdjustmentIn
            : InventoryTransactionType.AdjustmentOut;

        try
        {
            if (transactionType == InventoryTransactionType.AdjustmentIn)
            {
                balance.Increase(request.Quantity);
            }
            else
            {
                balance.Decrease(request.Quantity);
            }
        }
        catch (InvalidOperationException exception)
        {
            return Result<InventoryBalanceDto>.Fail("INVENTORY_NOT_AVAILABLE", exception.Message);
        }

        string actor = string.IsNullOrWhiteSpace(userName) ? DefaultSystemUser : userName.Trim();
        balance.MarkUpdated(actor);

        var transaction = new InventoryTransaction(
            request.ItemCode,
            request.WarehouseCode,
            request.LocationCode,
            request.LotNumber,
            request.Quantity,
            transactionType,
            request.ReasonCode,
            request.ReferenceType,
            request.ReferenceNumber,
            actor);

        await _inventoryRepository.UpsertBalanceAsync(balance, cancellationToken).ConfigureAwait(false);
        await _inventoryRepository.AddTransactionAsync(transaction, cancellationToken).ConfigureAwait(false);

        return Result<InventoryBalanceDto>.Ok(InventoryMapper.ToDto(balance), "Inventory adjusted successfully.");
    }

    public async Task<Result<InventoryReservationDto>> ReserveAsync(ReserveInventoryRequest request, string? userName, CancellationToken cancellationToken = default)
    {
        string validationError = ValidateReservationRequest(request);
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            return Result<InventoryReservationDto>.Fail("INVENTORY_VALIDATION", validationError);
        }

        InventoryBalance? balance = await _inventoryRepository
            .GetBalanceAsync(request.ItemCode, request.WarehouseCode, request.LocationCode, request.LotNumber, cancellationToken)
            .ConfigureAwait(false);

        if (balance is null)
        {
            return Result<InventoryReservationDto>.Fail("INVENTORY_NOT_FOUND", "No inventory balance was found for the requested item, warehouse, location and lot.");
        }

        try
        {
            balance.Reserve(request.Quantity);
        }
        catch (InvalidOperationException exception)
        {
            return Result<InventoryReservationDto>.Fail("INVENTORY_NOT_AVAILABLE", exception.Message);
        }

        string actor = string.IsNullOrWhiteSpace(userName) ? DefaultSystemUser : userName.Trim();
        var reservation = new InventoryReservation(
            request.ItemCode,
            request.WarehouseCode,
            request.LocationCode,
            request.LotNumber,
            request.Quantity,
            request.ReferenceType,
            request.ReferenceNumber,
            actor);

        var transaction = new InventoryTransaction(
            request.ItemCode,
            request.WarehouseCode,
            request.LocationCode,
            request.LotNumber,
            request.Quantity,
            InventoryTransactionType.PickingReserve,
            "RESERVE",
            request.ReferenceType,
            request.ReferenceNumber,
            actor);

        await _inventoryRepository.UpsertBalanceAsync(balance, cancellationToken).ConfigureAwait(false);
        await _inventoryRepository.AddReservationAsync(reservation, cancellationToken).ConfigureAwait(false);
        await _inventoryRepository.AddTransactionAsync(transaction, cancellationToken).ConfigureAwait(false);

        return Result<InventoryReservationDto>.Ok(InventoryMapper.ToDto(reservation), "Inventory reserved successfully.");
    }

    public async Task<Result<InventoryReservationDto>> ReleaseReservationAsync(ReleaseInventoryReservationRequest request, string? userName, CancellationToken cancellationToken = default)
    {
        if (request.ReservationId == Guid.Empty)
        {
            return Result<InventoryReservationDto>.Fail("INVENTORY_VALIDATION", "ReservationId is required.");
        }

        InventoryReservation? reservation = await _inventoryRepository.GetReservationAsync(request.ReservationId, cancellationToken).ConfigureAwait(false);
        if (reservation is null)
        {
            return Result<InventoryReservationDto>.Fail("RESERVATION_NOT_FOUND", "Inventory reservation was not found.");
        }

        InventoryBalance? balance = await _inventoryRepository
            .GetBalanceAsync(reservation.ItemCode, reservation.WarehouseCode, reservation.LocationCode, reservation.LotNumber, cancellationToken)
            .ConfigureAwait(false);

        if (balance is null)
        {
            return Result<InventoryReservationDto>.Fail("INVENTORY_NOT_FOUND", "Inventory balance for the reservation was not found.");
        }

        string actor = string.IsNullOrWhiteSpace(userName) ? DefaultSystemUser : userName.Trim();

        try
        {
            balance.Release(reservation.Quantity);
            reservation.Release(actor);
        }
        catch (InvalidOperationException exception)
        {
            return Result<InventoryReservationDto>.Fail("RESERVATION_INVALID_STATE", exception.Message);
        }

        var transaction = new InventoryTransaction(
            reservation.ItemCode,
            reservation.WarehouseCode,
            reservation.LocationCode,
            reservation.LotNumber,
            reservation.Quantity,
            InventoryTransactionType.ReservationRelease,
            "RELEASE",
            reservation.ReferenceType,
            reservation.ReferenceNumber,
            actor);

        await _inventoryRepository.UpsertBalanceAsync(balance, cancellationToken).ConfigureAwait(false);
        await _inventoryRepository.UpdateReservationAsync(reservation, cancellationToken).ConfigureAwait(false);
        await _inventoryRepository.AddTransactionAsync(transaction, cancellationToken).ConfigureAwait(false);

        return Result<InventoryReservationDto>.Ok(InventoryMapper.ToDto(reservation), "Inventory reservation released successfully.");
    }

    private static StockAvailabilityStatus GetStatus(decimal availableQuantity, decimal requestedQuantity)
    {
        if (availableQuantity >= requestedQuantity)
        {
            return StockAvailabilityStatus.Available;
        }

        return availableQuantity > 0 ? StockAvailabilityStatus.Partial : StockAvailabilityStatus.NotAvailable;
    }

    private static bool IsAdjustmentIn(string adjustmentType)
    {
        return adjustmentType.Equals("IN", StringComparison.OrdinalIgnoreCase)
            || adjustmentType.Equals("ENTRY", StringComparison.OrdinalIgnoreCase)
            || adjustmentType.Equals("AJUSTE_ENTRADA", StringComparison.OrdinalIgnoreCase);
    }

    private static string ValidateAvailabilityRequest(GetInventoryAvailabilityRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ItemCode)) return "ItemCode is required.";
        if (string.IsNullOrWhiteSpace(request.WarehouseCode)) return "WarehouseCode is required.";
        if (request.RequestedQuantity <= 0) return "RequestedQuantity must be greater than zero.";
        return string.Empty;
    }

    private static string ValidateAdjustmentRequest(AdjustInventoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ItemCode)) return "ItemCode is required.";
        if (string.IsNullOrWhiteSpace(request.WarehouseCode)) return "WarehouseCode is required.";
        if (string.IsNullOrWhiteSpace(request.LocationCode)) return "LocationCode is required.";
        if (request.Quantity <= 0) return "Quantity must be greater than zero.";
        if (string.IsNullOrWhiteSpace(request.AdjustmentType)) return "AdjustmentType is required.";
        if (!IsAdjustmentIn(request.AdjustmentType) && !request.AdjustmentType.Equals("OUT", StringComparison.OrdinalIgnoreCase) && !request.AdjustmentType.Equals("ISSUE", StringComparison.OrdinalIgnoreCase) && !request.AdjustmentType.Equals("AJUSTE_SALIDA", StringComparison.OrdinalIgnoreCase)) return "AdjustmentType must be IN or OUT.";
        if (string.IsNullOrWhiteSpace(request.ReasonCode)) return "ReasonCode is required.";
        return string.Empty;
    }

    private static string ValidateReservationRequest(ReserveInventoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ItemCode)) return "ItemCode is required.";
        if (string.IsNullOrWhiteSpace(request.WarehouseCode)) return "WarehouseCode is required.";
        if (string.IsNullOrWhiteSpace(request.LocationCode)) return "LocationCode is required.";
        if (request.Quantity <= 0) return "Quantity must be greater than zero.";
        if (string.IsNullOrWhiteSpace(request.ReferenceType)) return "ReferenceType is required.";
        if (string.IsNullOrWhiteSpace(request.ReferenceNumber)) return "ReferenceNumber is required.";
        return string.Empty;
    }

    private static string Normalize(string value) => value.Trim().ToUpperInvariant();

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
}
