using DETechOne.SmartWMS.Contracts.Requests.Audit;
using DETechOne.SmartWMS.Domain.Entities.Audit;

namespace DETechOne.SmartWMS.Application.Audit;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLogEntry entry, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<AuditLogEntry> Items, int TotalCount)> SearchAsync(AuditLogQueryRequest query, CancellationToken cancellationToken = default);
}
