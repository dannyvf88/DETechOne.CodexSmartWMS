using DETechOne.SmartWMS.Application.Picking;
using DETechOne.SmartWMS.Application.Security;
using DETechOne.SmartWMS.Contracts.Dtos.Picking;
using DETechOne.SmartWMS.Contracts.Requests.Picking;
using DETechOne.SmartWMS.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DETechOne.SmartWMS.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class PickingController : ControllerBase
{
    private readonly IPickingService _pickingService;
    private readonly IUserContextService _userContextService;

    public PickingController(IPickingService pickingService, IUserContextService userContextService)
    {
        _pickingService = pickingService;
        _userContextService = userContextService;
    }

    [HttpGet("open")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PickingDocumentDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PickingDocumentDto>>>> GetOpen(CancellationToken cancellationToken)
    {
        var result = await _pickingService.GetOpenAsync(cancellationToken).ConfigureAwait(false);
        return Ok(ApiResponse<IReadOnlyList<PickingDocumentDto>>.Ok(result.Value ?? Array.Empty<PickingDocumentDto>()));
    }

    [HttpGet("{pickingId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PickingDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PickingDocumentDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PickingDocumentDto>>> GetById(Guid pickingId, CancellationToken cancellationToken)
    {
        var result = await _pickingService.GetByIdAsync(pickingId, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return NotFound(ApiResponse<PickingDocumentDto>.Fail(result.ErrorCode ?? "PICKING_NOT_FOUND", result.Message ?? "Picking document was not found."));
        }

        return Ok(ApiResponse<PickingDocumentDto>.Ok(result.Value));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PickingDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PickingDocumentDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PickingDocumentDto>>> Create(CreatePickingRequest request, CancellationToken cancellationToken)
    {
        var result = await _pickingService.CreateAsync(request, _userContextService.UserName, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<PickingDocumentDto>.Fail(result.ErrorCode ?? "PICKING_ERROR", result.Message ?? "Picking document could not be created."));
        }

        return Ok(ApiResponse<PickingDocumentDto>.Ok(result.Value, result.Message));
    }

    [HttpPost("scan")]
    [ProducesResponseType(typeof(ApiResponse<PickingDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PickingDocumentDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PickingDocumentDto>>> Scan(ScanPickingItemRequest request, CancellationToken cancellationToken)
    {
        var result = await _pickingService.ScanItemAsync(request, _userContextService.UserName, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<PickingDocumentDto>.Fail(result.ErrorCode ?? "PICKING_ERROR", result.Message ?? "Picking scan could not be registered."));
        }

        return Ok(ApiResponse<PickingDocumentDto>.Ok(result.Value, result.Message));
    }

    [HttpPost("close")]
    [ProducesResponseType(typeof(ApiResponse<PickingDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PickingDocumentDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PickingDocumentDto>>> Close(ClosePickingRequest request, CancellationToken cancellationToken)
    {
        var result = await _pickingService.CloseAsync(request, _userContextService.UserName, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<PickingDocumentDto>.Fail(result.ErrorCode ?? "PICKING_ERROR", result.Message ?? "Picking document could not be closed."));
        }

        return Ok(ApiResponse<PickingDocumentDto>.Ok(result.Value, result.Message));
    }

    [HttpPost("cancel")]
    [ProducesResponseType(typeof(ApiResponse<PickingDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PickingDocumentDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PickingDocumentDto>>> Cancel(CancelPickingRequest request, CancellationToken cancellationToken)
    {
        var result = await _pickingService.CancelAsync(request, _userContextService.UserName, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<PickingDocumentDto>.Fail(result.ErrorCode ?? "PICKING_ERROR", result.Message ?? "Picking document could not be cancelled."));
        }

        return Ok(ApiResponse<PickingDocumentDto>.Ok(result.Value, result.Message));
    }
}
