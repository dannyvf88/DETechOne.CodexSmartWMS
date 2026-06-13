using DETechOne.SmartWMS.Domain.Entities.Packing;

namespace DETechOne.SmartWMS.Application.Packing;

public interface IPackingRepository
{
    Task<string> GetNextPackingNumberAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PackingDocument>> GetOpenAsync(CancellationToken cancellationToken = default);
    Task<PackingDocument?> GetByIdAsync(Guid packingId, CancellationToken cancellationToken = default);
    Task AddAsync(PackingDocument packingDocument, CancellationToken cancellationToken = default);
    Task UpdateAsync(PackingDocument packingDocument, CancellationToken cancellationToken = default);
}
