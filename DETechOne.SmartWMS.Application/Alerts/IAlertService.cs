using DETechOne.SmartWMS.Contracts.Dtos.Alerts;
using DETechOne.SmartWMS.Contracts.Requests.Alerts;
using DETechOne.SmartWMS.Contracts.Responses;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.Alerts;

public interface IAlertService
{
    Task<Result<OperationalAlertDto>> CreateAsync(CreateOperationalAlertRequest request, string userName, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<OperationalAlertDto>>> SearchAsync(AlertQueryRequest query, CancellationToken cancellationToken = default);
    Task<Result<OperationalAlertDto>> AcknowledgeAsync(Guid alertId, string userName, CancellationToken cancellationToken = default);
    Task<Result<OperationalAlertDto>> ResolveAsync(Guid alertId, string userName, CancellationToken cancellationToken = default);
}
