using DETechOne.SmartWMS.Application.EndToEnd;
using DETechOne.SmartWMS.Application.Security;
using DETechOne.SmartWMS.Contracts.Dtos.EndToEnd;
using DETechOne.SmartWMS.Contracts.Requests.EndToEnd;
using DETechOne.SmartWMS.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DETechOne.SmartWMS.API.Controllers;

[ApiController]
[Authorize]
[Route("api/end-to-end")]
public sealed class EndToEndController : ControllerBase
{
    private readonly IEndToEndFlowOrchestrator _orchestrator;
    private readonly IUserContextService _userContextService;

    public EndToEndController(
        IEndToEndFlowOrchestrator orchestrator,
        IUserContextService userContextService)
    {
        _orchestrator = orchestrator;
        _userContextService = userContextService;
    }

    [HttpPost("order-to-delivery/start")]
    [ProducesResponseType(typeof(ApiResponse<OrderToDeliveryFlowDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<OrderToDeliveryFlowDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<OrderToDeliveryFlowDto>>> StartOrderToDeliveryFlow(
        StartOrderToDeliveryFlowRequest request,
        CancellationToken cancellationToken)
    {
        string? userName = _userContextService.UserName;

        if (string.IsNullOrWhiteSpace(userName))
        {
            return Unauthorized();
        }

        var result = await _orchestrator
            .StartOrderToDeliveryFlowAsync(request, userName, cancellationToken)
            .ConfigureAwait(false);

        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<OrderToDeliveryFlowDto>.Fail(
                result.ErrorCode ?? "E2E_START_ERROR",
                result.Message ?? "End-to-end flow could not be started."));
        }

        return Ok(ApiResponse<OrderToDeliveryFlowDto>.Ok(result.Value, result.Message));
    }

    [HttpPost("order-to-delivery/execute")]
    [ProducesResponseType(typeof(ApiResponse<OrderToDeliveryFlowDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<OrderToDeliveryFlowDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<OrderToDeliveryFlowDto>>> ExecuteOrderToDeliveryFlow(
        ExecuteOrderToDeliveryFlowRequest request,
        CancellationToken cancellationToken)
    {
        string? userName = _userContextService.UserName;

        if (string.IsNullOrWhiteSpace(userName))
        {
            return Unauthorized();
        }

        var result = await _orchestrator
            .ExecuteAsync(request, userName, cancellationToken)
            .ConfigureAwait(false);

        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<OrderToDeliveryFlowDto>.Fail(
                result.ErrorCode ?? "E2E_EXECUTION_ERROR",
                result.Message ?? "End-to-end flow could not be executed."));
        }

        return Ok(ApiResponse<OrderToDeliveryFlowDto>.Ok(result.Value, result.Message));
    }

    [HttpGet("order-to-delivery/{flowId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<OrderToDeliveryFlowDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<OrderToDeliveryFlowDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<OrderToDeliveryFlowDto>>> GetOrderToDeliveryFlow(
        Guid flowId,
        CancellationToken cancellationToken)
    {
        var result = await _orchestrator
            .GetByIdAsync(flowId, cancellationToken)
            .ConfigureAwait(false);

        if (!result.Success || result.Value is null)
        {
            return NotFound(ApiResponse<OrderToDeliveryFlowDto>.Fail(
                result.ErrorCode ?? "E2E_NOT_FOUND",
                result.Message ?? "End-to-end flow was not found."));
        }

        return Ok(ApiResponse<OrderToDeliveryFlowDto>.Ok(result.Value, result.Message));
    }
}
