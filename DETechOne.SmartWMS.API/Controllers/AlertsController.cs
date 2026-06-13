using DETechOne.SmartWMS.Application.Alerts;
using DETechOne.SmartWMS.Application.Security;
using DETechOne.SmartWMS.Contracts.Dtos.Alerts;
using DETechOne.SmartWMS.Contracts.Requests.Alerts;
using DETechOne.SmartWMS.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DETechOne.SmartWMS.API.Controllers;

[ApiController]
[Route("api/alerts")]
public sealed class AlertsController : ControllerBase
{
    private readonly IAlertService _alertService;
    private readonly IUserContextService _userContextService;

    public AlertsController(IAlertService alertService, IUserContextService userContextService)
    {
        _alertService = alertService;
        _userContextService = userContextService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OperationalAlertDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<OperationalAlertDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<OperationalAlertDto>>> Create(
        CreateOperationalAlertRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _alertService
            .CreateAsync(request, ResolveUserName(), cancellationToken)
            .ConfigureAwait(false);

        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<OperationalAlertDto>.Fail(
                result.ErrorCode ?? "ALERT_CREATE_ERROR",
                result.Message ?? "Operational alert could not be created."));
        }

        return Ok(ApiResponse<OperationalAlertDto>.Ok(result.Value, result.Message));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OperationalAlertDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<OperationalAlertDto>>>> Search(
        [FromQuery] AlertQueryRequest query,
        CancellationToken cancellationToken)
    {
        var result = await _alertService.SearchAsync(query, cancellationToken).ConfigureAwait(false);

        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<PagedResult<OperationalAlertDto>>.Fail(
                result.ErrorCode ?? "ALERT_SEARCH_ERROR",
                result.Message ?? "Operational alerts could not be retrieved."));
        }

        return Ok(ApiResponse<PagedResult<OperationalAlertDto>>.Ok(result.Value, result.Message));
    }

    [HttpPost("{alertId:guid}/acknowledge")]
    [ProducesResponseType(typeof(ApiResponse<OperationalAlertDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<OperationalAlertDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<OperationalAlertDto>>> Acknowledge(
        Guid alertId,
        CancellationToken cancellationToken)
    {
        var result = await _alertService
            .AcknowledgeAsync(alertId, ResolveUserName(), cancellationToken)
            .ConfigureAwait(false);

        if (!result.Success || result.Value is null)
        {
            return NotFound(ApiResponse<OperationalAlertDto>.Fail(
                result.ErrorCode ?? "ALERT_NOT_FOUND",
                result.Message ?? "Operational alert was not found."));
        }

        return Ok(ApiResponse<OperationalAlertDto>.Ok(result.Value, result.Message));
    }

    [HttpPost("{alertId:guid}/resolve")]
    [ProducesResponseType(typeof(ApiResponse<OperationalAlertDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<OperationalAlertDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<OperationalAlertDto>>> Resolve(
        Guid alertId,
        CancellationToken cancellationToken)
    {
        var result = await _alertService
            .ResolveAsync(alertId, ResolveUserName(), cancellationToken)
            .ConfigureAwait(false);

        if (!result.Success || result.Value is null)
        {
            return NotFound(ApiResponse<OperationalAlertDto>.Fail(
                result.ErrorCode ?? "ALERT_NOT_FOUND",
                result.Message ?? "Operational alert was not found."));
        }

        return Ok(ApiResponse<OperationalAlertDto>.Ok(result.Value, result.Message));
    }

    private string ResolveUserName()
    {
        return string.IsNullOrWhiteSpace(_userContextService.UserName)
            ? "SmartWMS.API"
            : _userContextService.UserName;
    }
}
