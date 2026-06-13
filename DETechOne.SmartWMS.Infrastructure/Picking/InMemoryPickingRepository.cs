using DETechOne.SmartWMS.Application.Picking;
using DETechOne.SmartWMS.Domain.Entities.Picking;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Infrastructure.Picking;

public sealed class InMemoryPickingRepository : IPickingRepository
{
    private readonly object _syncRoot = new();
    private readonly List<PickingDocument> _pickings = new();
    private int _sequence;

    public Task<string> GetNextPickingNumberAsync(CancellationToken cancellationToken = default)
    {
        lock (_syncRoot)
        {
            _sequence++;
            return Task.FromResult($"PICK-{DateTime.UtcNow:yyyyMMdd}-{_sequence:000000}");
        }
    }

    public Task<IReadOnlyList<PickingDocument>> GetOpenAsync(CancellationToken cancellationToken = default)
    {
        lock (_syncRoot)
        {
            IReadOnlyList<PickingDocument> result = _pickings
                .Where(picking => picking.Status is PickingStatus.Open or PickingStatus.InProgress)
                .OrderByDescending(picking => picking.CreatedAtUtc)
                .ToArray();

            return Task.FromResult(result);
        }
    }

    public Task<PickingDocument?> GetByIdAsync(Guid pickingId, CancellationToken cancellationToken = default)
    {
        lock (_syncRoot)
        {
            PickingDocument? result = _pickings.FirstOrDefault(picking => picking.Id == pickingId);
            return Task.FromResult(result);
        }
    }

    public Task AddAsync(PickingDocument pickingDocument, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pickingDocument);

        lock (_syncRoot)
        {
            _pickings.Add(pickingDocument);
            return Task.CompletedTask;
        }
    }

    public Task UpdateAsync(PickingDocument pickingDocument, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pickingDocument);
        return Task.CompletedTask;
    }
}
