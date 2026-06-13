using DETechOne.SmartWMS.Contracts.Dtos.Dashboard;
using DETechOne.SmartWMS.Contracts.Requests.Dashboard;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.Dashboard;

public interface IDashboardMetricsService
{
    Task<Result<DashboardOverviewDto>> GetOverviewAsync(DashboardMetricsRequest request, CancellationToken cancellationToken = default);
}
