using System.Collections.Concurrent;
using DETechOne.SmartWMS.Application.Shipping;
using DETechOne.SmartWMS.Domain.Entities.Shipping;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Infrastructure.Shipping;

public sealed class InMemoryShippingRepository : IShippingRepository
{
    private readonly ConcurrentDictionary<Guid, ShippingDocument> _shippings = new();
    private int _sequence;

    public Task<string> GetNextShippingNumberAsync(CancellationToken cancellationToken = default)
    {
        int next = Interlocked.Increment(ref _sequence);
        return Task.FromResult($"SHP-{DateTime.UtcNow:yyyyMMdd}-{next:000000}");
    }

    public Task<IReadOnlyList<ShippingDocument>> GetOpenAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ShippingDocument> result = _shippings.Values
            .Where(shipping => shipping.Status is ShippingStatus.Open or ShippingStatus.InProgress or ShippingStatus.Confirmed)
            .OrderByDescending(shipping => shipping.CreatedAtUtc)
            .ToArray();

        return Task.FromResult(result);
    }

    public Task<ShippingDocument?> GetByIdAsync(Guid shippingId, CancellationToken cancellationToken = default)
    {
        _shippings.TryGetValue(shippingId, out ShippingDocument? shipping);
        return Task.FromResult(shipping);
    }

    public Task AddAsync(ShippingDocument shippingDocument, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(shippingDocument);

        if (!_shippings.TryAdd(shippingDocument.Id, shippingDocument))
        {
            throw new InvalidOperationException($"Shipping document {shippingDocument.Id} already exists.");
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(ShippingDocument shippingDocument, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(shippingDocument);
        _shippings[shippingDocument.Id] = shippingDocument;
        return Task.CompletedTask;
    }
}
