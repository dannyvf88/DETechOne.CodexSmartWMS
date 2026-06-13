using System.Collections.Concurrent;
using DETechOne.SmartWMS.Application.Movement;
using DETechOne.SmartWMS.Domain.Entities.Movement;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Infrastructure.Movement;

public sealed class InMemoryMovementRepository : IMovementRepository
{
    private readonly ConcurrentDictionary<Guid, MovementDocument> _documents = new();
    private int _sequence;

    public Task<MovementDocument?> GetByIdAsync(Guid movementId, CancellationToken cancellationToken = default)
    {
        _documents.TryGetValue(movementId, out MovementDocument? document);
        return Task.FromResult(document);
    }

    public Task<IReadOnlyList<MovementDocument>> GetOpenAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<MovementDocument> documents = _documents.Values
            .Where(document => document.Status == MovementStatus.Open)
            .OrderByDescending(document => document.CreatedAtUtc)
            .ToArray();

        return Task.FromResult(documents);
    }

    public Task AddAsync(MovementDocument movementDocument, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(movementDocument);
        _documents[movementDocument.Id] = movementDocument;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(MovementDocument movementDocument, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(movementDocument);
        _documents[movementDocument.Id] = movementDocument;
        return Task.CompletedTask;
    }

    public Task<string> GetNextMovementNumberAsync(MovementType movementType, CancellationToken cancellationToken = default)
    {
        int next = Interlocked.Increment(ref _sequence);
        string prefix = movementType switch
        {
            MovementType.Transfer => "MOV-TRF",
            MovementType.Replenishment => "MOV-REP",
            MovementType.Relocation => "MOV-REL",
            MovementType.PickingMove => "MOV-PCK",
            MovementType.PackingMove => "MOV-PKG",
            MovementType.ShippingMove => "MOV-SHP",
            _ => "MOV"
        };

        return Task.FromResult($"{prefix}-{DateTime.UtcNow:yyyyMMdd}-{next:000000}");
    }
}
