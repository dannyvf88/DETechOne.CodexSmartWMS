using DETechOne.SmartWMS.Application.Movement;
using DETechOne.SmartWMS.Application.Security;
using DETechOne.SmartWMS.Contracts.Dtos.Movement;
using DETechOne.SmartWMS.Contracts.Requests.Movement;
using DETechOne.SmartWMS.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DETechOne.SmartWMS.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class MovementController : ControllerBase
{
    private readonly IMovementService _movementService;
    private readonly IUserContextService _userContextService;

    public MovementController(IMovementService movementService, IUserContextService userContextService)
    {
        _movementService = movementService;
        _userContextService = userContextService;
    }

    [HttpGet("open")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<MovementDocumentDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<MovementDocumentDto>>>> GetOpen(CancellationToken cancellationToken)
    {
        var result = await _movementService.GetOpenAsync(cancellationToken).ConfigureAwait(false);
        return Ok(ApiResponse<IReadOnlyList<MovementDocumentDto>>.Ok(result.Value ?? Array.Empty<MovementDocumentDto>()));
    }

    [HttpGet("{movementId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<MovementDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<MovementDocumentDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<MovementDocumentDto>>> GetById(Guid movementId, CancellationToken cancellationToken)
    {
        var result = await _movementService.GetByIdAsync(movementId, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return NotFound(ApiResponse<MovementDocumentDto>.Fail(result.ErrorCode ?? "MOVEMENT_NOT_FOUND", result.Message ?? "Movement document was not found."));
        }

        return Ok(ApiResponse<MovementDocumentDto>.Ok(result.Value));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MovementDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<MovementDocumentDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<MovementDocumentDto>>> Create(CreateMovementRequest request, CancellationToken cancellationToken)
    {
        var result = await _movementService.CreateAsync(request, _userContextService.UserName, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<MovementDocumentDto>.Fail(result.ErrorCode ?? "MOVEMENT_ERROR", result.Message ?? "Movement document could not be created."));
        }

        return Ok(ApiResponse<MovementDocumentDto>.Ok(result.Value, result.Message));
    }

    [HttpPost("confirm")]
    [ProducesResponseType(typeof(ApiResponse<MovementDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<MovementDocumentDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<MovementDocumentDto>>> Confirm(ConfirmMovementRequest request, CancellationToken cancellationToken)
    {
        var result = await _movementService.ConfirmAsync(request, _userContextService.UserName, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<MovementDocumentDto>.Fail(result.ErrorCode ?? "MOVEMENT_ERROR", result.Message ?? "Movement document could not be confirmed."));
        }

        return Ok(ApiResponse<MovementDocumentDto>.Ok(result.Value, result.Message));
    }

    [HttpPost("cancel")]
    [ProducesResponseType(typeof(ApiResponse<MovementDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<MovementDocumentDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<MovementDocumentDto>>> Cancel(CancelMovementRequest request, CancellationToken cancellationToken)
    {
        var result = await _movementService.CancelAsync(request, _userContextService.UserName, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<MovementDocumentDto>.Fail(result.ErrorCode ?? "MOVEMENT_ERROR", result.Message ?? "Movement document could not be cancelled."));
        }

        return Ok(ApiResponse<MovementDocumentDto>.Ok(result.Value, result.Message));
    }
}
