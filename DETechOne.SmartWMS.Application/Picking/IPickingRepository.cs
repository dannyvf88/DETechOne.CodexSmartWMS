using DETechOne.SmartWMS.Domain.Entities.Picking;

namespace DETechOne.SmartWMS.Application.Picking;

public interface IPickingRepository
{
    Task<string> GetNextPickingNumberAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PickingDocument>> GetOpenAsync(CancellationToken cancellationToken = default);
    Task<PickingDocument?> GetByIdAsync(Guid pickingId, CancellationToken cancellationToken = default);
    Task AddAsync(PickingDocument pickingDocument, CancellationToken cancellationToken = default);
    Task UpdateAsync(PickingDocument pickingDocument, CancellationToken cancellationToken = default);
}
