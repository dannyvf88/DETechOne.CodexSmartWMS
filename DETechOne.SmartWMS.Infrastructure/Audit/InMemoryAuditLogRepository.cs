using System.Collections.Concurrent;
using DETechOne.SmartWMS.Application.Audit;
using DETechOne.SmartWMS.Contracts.Requests.Audit;
using DETechOne.SmartWMS.Domain.Entities.Audit;

namespace DETechOne.SmartWMS.Infrastructure.Audit;

public sealed class InMemoryAuditLogRepository : IAuditLogRepository
{
    private readonly ConcurrentDictionary<Guid, AuditLogEntry> _entries = new();

    public Task AddAsync(AuditLogEntry entry, CancellationToken cancellationToken = default)
    {
        _entries[entry.Id] = entry;
        return Task.CompletedTask;
    }

    public Task<(IReadOnlyList<AuditLogEntry> Items, int TotalCount)> SearchAsync(AuditLogQueryRequest query, CancellationToken cancellationToken = default)
    {
        IEnumerable<AuditLogEntry> result = _entries.Values;

        result = Filter(result, query);

        int totalCount = result.Count();
        IReadOnlyList<AuditLogEntry> items = result
            .OrderByDescending(x => x.OccurredAtUtc)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToArray();

        return Task.FromResult((items, totalCount));
    }

    private static IEnumerable<AuditLogEntry> Filter(IEnumerable<AuditLogEntry> source, AuditLogQueryRequest query)
    {
        if (!string.IsNullOrWhiteSpace(query.Module))
        {
            source = source.Where(x => string.Equals(x.Module, query.Module, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.Action))
        {
            source = source.Where(x => string.Equals(x.Action, query.Action, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.EntityType))
        {
            source = source.Where(x => string.Equals(x.EntityType, query.EntityType, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.EntityId))
        {
            source = source.Where(x => string.Equals(x.EntityId, query.EntityId, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.UserName))
        {
            source = source.Where(x => string.Equals(x.UserName, query.UserName, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.DeviceCode))
        {
            source = source.Where(x => string.Equals(x.DeviceCode, query.DeviceCode, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.CorrelationId))
        {
            source = source.Where(x => string.Equals(x.CorrelationId, query.CorrelationId, StringComparison.OrdinalIgnoreCase));
        }

        if (query.FromUtc.HasValue)
        {
            source = source.Where(x => x.OccurredAtUtc >= query.FromUtc.Value);
        }

        if (query.ToUtc.HasValue)
        {
            source = source.Where(x => x.OccurredAtUtc <= query.ToUtc.Value);
        }

        return source;
    }
}
