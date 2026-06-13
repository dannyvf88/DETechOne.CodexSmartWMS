using DETechOne.SmartWMS.Contracts.Requests.Alerts;
using DETechOne.SmartWMS.Domain.Entities.Alerts;

namespace DETechOne.SmartWMS.Application.Alerts;

public interface IAlertRepository
{
    Task AddAsync(OperationalAlert alert, CancellationToken cancellationToken = default);
    Task<OperationalAlert?> GetByIdAsync(Guid alertId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<OperationalAlert> Items, int TotalCount)> SearchAsync(AlertQueryRequest query, CancellationToken cancellationToken = default);
    Task UpdateAsync(OperationalAlert alert, CancellationToken cancellationToken = default);
}
