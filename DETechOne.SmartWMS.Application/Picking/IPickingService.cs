using DETechOne.SmartWMS.Contracts.Dtos.Picking;
using DETechOne.SmartWMS.Contracts.Requests.Picking;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.Picking;

public interface IPickingService
{
    Task<Result<IReadOnlyList<PickingDocumentDto>>> GetOpenAsync(CancellationToken cancellationToken = default);
    Task<Result<PickingDocumentDto>> GetByIdAsync(Guid pickingId, CancellationToken cancellationToken = default);
    Task<Result<PickingDocumentDto>> CreateAsync(CreatePickingRequest request, string? userName, CancellationToken cancellationToken = default);
    Task<Result<PickingDocumentDto>> ScanItemAsync(ScanPickingItemRequest request, string? userName, CancellationToken cancellationToken = default);
    Task<Result<PickingDocumentDto>> CloseAsync(ClosePickingRequest request, string? userName, CancellationToken cancellationToken = default);
    Task<Result<PickingDocumentDto>> CancelAsync(CancelPickingRequest request, string? userName, CancellationToken cancellationToken = default);
}
