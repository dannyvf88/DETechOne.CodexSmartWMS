using System.Collections.Concurrent;
using DETechOne.SmartWMS.Application.Packing;
using DETechOne.SmartWMS.Domain.Entities.Packing;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Infrastructure.Packing;

public sealed class InMemoryPackingRepository : IPackingRepository
{
    private readonly ConcurrentDictionary<Guid, PackingDocument> _packings = new();
    private int _sequence;

    public Task<string> GetNextPackingNumberAsync(CancellationToken cancellationToken = default)
    {
        int next = Interlocked.Increment(ref _sequence);
        return Task.FromResult($"PKG-{DateTime.UtcNow:yyyyMMdd}-{next:000000}");
    }

    public Task<IReadOnlyList<PackingDocument>> GetOpenAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<PackingDocument> result = _packings.Values
            .Where(packing => packing.Status is PackingStatus.Open or PackingStatus.InProgress)
            .OrderByDescending(packing => packing.CreatedAtUtc)
            .ToArray();

        return Task.FromResult(result);
    }

    public Task<PackingDocument?> GetByIdAsync(Guid packingId, CancellationToken cancellationToken = default)
    {
        _packings.TryGetValue(packingId, out PackingDocument? packing);
        return Task.FromResult(packing);
    }

    public Task AddAsync(PackingDocument packingDocument, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(packingDocument);

        if (!_packings.TryAdd(packingDocument.Id, packingDocument))
        {
            throw new InvalidOperationException($"Packing document {packingDocument.Id} already exists.");
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(PackingDocument packingDocument, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(packingDocument);
        _packings[packingDocument.Id] = packingDocument;
        return Task.CompletedTask;
    }
}
