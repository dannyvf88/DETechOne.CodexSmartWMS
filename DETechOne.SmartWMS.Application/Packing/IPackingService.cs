using DETechOne.SmartWMS.Contracts.Dtos.Packing;
using DETechOne.SmartWMS.Contracts.Requests.Packing;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.Packing;

public interface IPackingService
{
    Task<Result<IReadOnlyList<PackingDocumentDto>>> GetOpenAsync(CancellationToken cancellationToken = default);
    Task<Result<PackingDocumentDto>> GetByIdAsync(Guid packingId, CancellationToken cancellationToken = default);
    Task<Result<PackingDocumentDto>> CreateAsync(CreatePackingRequest request, string? userName, CancellationToken cancellationToken = default);
    Task<Result<PackingDocumentDto>> PackItemAsync(PackItemRequest request, string? userName, CancellationToken cancellationToken = default);
    Task<Result<PackingDocumentDto>> CloseAsync(ClosePackingRequest request, string? userName, CancellationToken cancellationToken = default);
    Task<Result<PackingDocumentDto>> CancelAsync(CancelPackingRequest request, string? userName, CancellationToken cancellationToken = default);
}
