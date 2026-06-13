using System.Collections.Concurrent;
using DETechOne.SmartWMS.Application.Alerts;
using DETechOne.SmartWMS.Contracts.Requests.Alerts;
using DETechOne.SmartWMS.Domain.Entities.Alerts;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Infrastructure.Alerts;

public sealed class InMemoryAlertRepository : IAlertRepository
{
    private readonly ConcurrentDictionary<Guid, OperationalAlert> _alerts = new();

    public Task AddAsync(OperationalAlert alert, CancellationToken cancellationToken = default)
    {
        _alerts[alert.Id] = alert;
        return Task.CompletedTask;
    }

    public Task<OperationalAlert?> GetByIdAsync(Guid alertId, CancellationToken cancellationToken = default)
    {
        _alerts.TryGetValue(alertId, out OperationalAlert? alert);
        return Task.FromResult(alert);
    }

    public Task<(IReadOnlyList<OperationalAlert> Items, int TotalCount)> SearchAsync(AlertQueryRequest query, CancellationToken cancellationToken = default)
    {
        IEnumerable<OperationalAlert> result = _alerts.Values;

        result = Filter(result, query);

        int totalCount = result.Count();
        IReadOnlyList<OperationalAlert> items = result
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToArray();

        return Task.FromResult((items, totalCount));
    }

    public Task UpdateAsync(OperationalAlert alert, CancellationToken cancellationToken = default)
    {
        _alerts[alert.Id] = alert;
        return Task.CompletedTask;
    }

    private static IEnumerable<OperationalAlert> Filter(IEnumerable<OperationalAlert> source, AlertQueryRequest query)
    {
        if (!string.IsNullOrWhiteSpace(query.Severity) && Enum.TryParse(query.Severity, true, out AlertSeverity severity))
        {
            source = source.Where(x => x.Severity == severity);
        }

        if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse(query.Status, true, out AlertStatus status))
        {
            source = source.Where(x => x.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(query.Source))
        {
            source = source.Where(x => string.Equals(x.Source, query.Source, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.Code))
        {
            source = source.Where(x => string.Equals(x.Code, query.Code, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.EntityType))
        {
            source = source.Where(x => string.Equals(x.EntityType, query.EntityType, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.EntityId))
        {
            source = source.Where(x => string.Equals(x.EntityId, query.EntityId, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.DeviceCode))
        {
            source = source.Where(x => string.Equals(x.DeviceCode, query.DeviceCode, StringComparison.OrdinalIgnoreCase));
        }

        if (query.FromUtc.HasValue)
        {
            source = source.Where(x => x.CreatedAtUtc >= query.FromUtc.Value);
        }

        if (query.ToUtc.HasValue)
        {
            source = source.Where(x => x.CreatedAtUtc <= query.ToUtc.Value);
        }

        return source;
    }
}
