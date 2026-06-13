using DETechOne.SmartWMS.Contracts.Dtos.Inventory;
using DETechOne.SmartWMS.Contracts.Requests.Inventory;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.Inventory;

public interface IInventoryService
{
    Task<Result<InventoryAvailabilityDto>> GetAvailabilityAsync(GetInventoryAvailabilityRequest request, CancellationToken cancellationToken = default);
    Task<Result<InventoryBalanceDto>> AdjustAsync(AdjustInventoryRequest request, string? userName, CancellationToken cancellationToken = default);
    Task<Result<InventoryReservationDto>> ReserveAsync(ReserveInventoryRequest request, string? userName, CancellationToken cancellationToken = default);
    Task<Result<InventoryReservationDto>> ReleaseReservationAsync(ReleaseInventoryReservationRequest request, string? userName, CancellationToken cancellationToken = default);
}
