using DETechOne.SmartWMS.Application.Dashboard;
using DETechOne.SmartWMS.Contracts.Dtos.Dashboard;
using DETechOne.SmartWMS.Contracts.Requests.Dashboard;
using DETechOne.SmartWMS.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DETechOne.SmartWMS.API.Controllers;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardMetricsService _dashboardMetricsService;

    public DashboardController(IDashboardMetricsService dashboardMetricsService)
    {
        _dashboardMetricsService = dashboardMetricsService;
    }

    [HttpGet("overview")]
    [ProducesResponseType(typeof(ApiResponse<DashboardOverviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<DashboardOverviewDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<DashboardOverviewDto>>> GetOverview(
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        [FromQuery] string? warehouseCode,
        CancellationToken cancellationToken)
    {
        var request = new DashboardMetricsRequest
        {
            FromUtc = fromUtc,
            ToUtc = toUtc,
            WarehouseCode = warehouseCode
        };

        var result = await _dashboardMetricsService.GetOverviewAsync(request, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<DashboardOverviewDto>.Fail(
                result.ErrorCode ?? "DASHBOARD_METRICS_ERROR",
                result.Message ?? "Dashboard metrics could not be generated."));
        }

        return Ok(ApiResponse<DashboardOverviewDto>.Ok(result.Value, result.Message));
    }
}
