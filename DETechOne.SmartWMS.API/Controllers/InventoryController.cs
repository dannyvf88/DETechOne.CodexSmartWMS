using DETechOne.SmartWMS.Application.Inventory;
using DETechOne.SmartWMS.Application.Security;
using DETechOne.SmartWMS.Contracts.Dtos.Inventory;
using DETechOne.SmartWMS.Contracts.Requests.Inventory;
using DETechOne.SmartWMS.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DETechOne.SmartWMS.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    private readonly IUserContextService _userContextService;

    public InventoryController(IInventoryService inventoryService, IUserContextService userContextService)
    {
        _inventoryService = inventoryService;
        _userContextService = userContextService;
    }

    [HttpGet("availability")]
    [ProducesResponseType(typeof(ApiResponse<InventoryAvailabilityDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<InventoryAvailabilityDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<InventoryAvailabilityDto>>> GetAvailability([FromQuery] GetInventoryAvailabilityRequest request, CancellationToken cancellationToken)
    {
        var result = await _inventoryService.GetAvailabilityAsync(request, cancellationToken).ConfigureAwait(false);

        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<InventoryAvailabilityDto>.Fail(result.ErrorCode ?? "INVENTORY_ERROR", result.Message ?? "Inventory availability could not be calculated."));
        }

        return Ok(ApiResponse<InventoryAvailabilityDto>.Ok(result.Value));
    }

    [HttpPost("adjustments")]
    [ProducesResponseType(typeof(ApiResponse<InventoryBalanceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<InventoryBalanceDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<InventoryBalanceDto>>> Adjust(AdjustInventoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _inventoryService.AdjustAsync(request, _userContextService.UserName, cancellationToken).ConfigureAwait(false);

        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<InventoryBalanceDto>.Fail(result.ErrorCode ?? "INVENTORY_ERROR", result.Message ?? "Inventory could not be adjusted."));
        }

        return Ok(ApiResponse<InventoryBalanceDto>.Ok(result.Value, result.Message));
    }

    [HttpPost("reservations")]
    [ProducesResponseType(typeof(ApiResponse<InventoryReservationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<InventoryReservationDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<InventoryReservationDto>>> Reserve(ReserveInventoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _inventoryService.ReserveAsync(request, _userContextService.UserName, cancellationToken).ConfigureAwait(false);

        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<InventoryReservationDto>.Fail(result.ErrorCode ?? "INVENTORY_ERROR", result.Message ?? "Inventory could not be reserved."));
        }

        return Ok(ApiResponse<InventoryReservationDto>.Ok(result.Value, result.Message));
    }

    [HttpPost("reservations/release")]
    [ProducesResponseType(typeof(ApiResponse<InventoryReservationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<InventoryReservationDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<InventoryReservationDto>>> ReleaseReservation(ReleaseInventoryReservationRequest request, CancellationToken cancellationToken)
    {
        var result = await _inventoryService.ReleaseReservationAsync(request, _userContextService.UserName, cancellationToken).ConfigureAwait(false);

        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<InventoryReservationDto>.Fail(result.ErrorCode ?? "INVENTORY_ERROR", result.Message ?? "Inventory reservation could not be released."));
        }

        return Ok(ApiResponse<InventoryReservationDto>.Ok(result.Value, result.Message));
    }
}
