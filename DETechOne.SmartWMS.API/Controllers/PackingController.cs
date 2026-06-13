using DETechOne.SmartWMS.Application.Packing;
using DETechOne.SmartWMS.Application.Security;
using DETechOne.SmartWMS.Contracts.Dtos.Packing;
using DETechOne.SmartWMS.Contracts.Requests.Packing;
using DETechOne.SmartWMS.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DETechOne.SmartWMS.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class PackingController : ControllerBase
{
    private readonly IPackingService _packingService;
    private readonly IUserContextService _userContextService;

    public PackingController(IPackingService packingService, IUserContextService userContextService)
    {
        _packingService = packingService;
        _userContextService = userContextService;
    }

    [HttpGet("open")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PackingDocumentDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PackingDocumentDto>>>> GetOpen(CancellationToken cancellationToken)
    {
        var result = await _packingService.GetOpenAsync(cancellationToken).ConfigureAwait(false);
        return Ok(ApiResponse<IReadOnlyList<PackingDocumentDto>>.Ok(result.Value ?? Array.Empty<PackingDocumentDto>()));
    }

    [HttpGet("{packingId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PackingDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PackingDocumentDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PackingDocumentDto>>> GetById(Guid packingId, CancellationToken cancellationToken)
    {
        var result = await _packingService.GetByIdAsync(packingId, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return NotFound(ApiResponse<PackingDocumentDto>.Fail(result.ErrorCode ?? "PACKING_NOT_FOUND", result.Message ?? "Packing document was not found."));
        }

        return Ok(ApiResponse<PackingDocumentDto>.Ok(result.Value));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PackingDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PackingDocumentDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PackingDocumentDto>>> Create(CreatePackingRequest request, CancellationToken cancellationToken)
    {
        var result = await _packingService.CreateAsync(request, _userContextService.UserName, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<PackingDocumentDto>.Fail(result.ErrorCode ?? "PACKING_ERROR", result.Message ?? "Packing document could not be created."));
        }

        return Ok(ApiResponse<PackingDocumentDto>.Ok(result.Value, result.Message));
    }

    [HttpPost("pack")]
    [ProducesResponseType(typeof(ApiResponse<PackingDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PackingDocumentDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PackingDocumentDto>>> Pack(PackItemRequest request, CancellationToken cancellationToken)
    {
        var result = await _packingService.PackItemAsync(request, _userContextService.UserName, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<PackingDocumentDto>.Fail(result.ErrorCode ?? "PACKING_ERROR", result.Message ?? "Packing scan could not be registered."));
        }

        return Ok(ApiResponse<PackingDocumentDto>.Ok(result.Value, result.Message));
    }

    [HttpPost("close")]
    [ProducesResponseType(typeof(ApiResponse<PackingDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PackingDocumentDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PackingDocumentDto>>> Close(ClosePackingRequest request, CancellationToken cancellationToken)
    {
        var result = await _packingService.CloseAsync(request, _userContextService.UserName, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<PackingDocumentDto>.Fail(result.ErrorCode ?? "PACKING_ERROR", result.Message ?? "Packing document could not be closed."));
        }

        return Ok(ApiResponse<PackingDocumentDto>.Ok(result.Value, result.Message));
    }

    [HttpPost("cancel")]
    [ProducesResponseType(typeof(ApiResponse<PackingDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PackingDocumentDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PackingDocumentDto>>> Cancel(CancelPackingRequest request, CancellationToken cancellationToken)
    {
        var result = await _packingService.CancelAsync(request, _userContextService.UserName, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<PackingDocumentDto>.Fail(result.ErrorCode ?? "PACKING_ERROR", result.Message ?? "Packing document could not be cancelled."));
        }

        return Ok(ApiResponse<PackingDocumentDto>.Ok(result.Value, result.Message));
    }
}
