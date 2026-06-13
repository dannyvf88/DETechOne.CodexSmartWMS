using DETechOne.SmartWMS.Application.Security;
using DETechOne.SmartWMS.Application.Shipping;
using DETechOne.SmartWMS.Contracts.Dtos.Shipping;
using DETechOne.SmartWMS.Contracts.Requests.Shipping;
using DETechOne.SmartWMS.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DETechOne.SmartWMS.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class ShippingController : ControllerBase
{
    private readonly IShippingService _shippingService;
    private readonly IUserContextService _userContextService;

    public ShippingController(IShippingService shippingService, IUserContextService userContextService)
    {
        _shippingService = shippingService;
        _userContextService = userContextService;
    }

    [HttpGet("open")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ShippingDocumentDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ShippingDocumentDto>>>> GetOpen(CancellationToken cancellationToken)
    {
        var result = await _shippingService.GetOpenAsync(cancellationToken).ConfigureAwait(false);
        return Ok(ApiResponse<IReadOnlyList<ShippingDocumentDto>>.Ok(result.Value ?? Array.Empty<ShippingDocumentDto>()));
    }

    [HttpGet("{shippingId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ShippingDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ShippingDocumentDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ShippingDocumentDto>>> GetById(Guid shippingId, CancellationToken cancellationToken)
    {
        var result = await _shippingService.GetByIdAsync(shippingId, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return NotFound(ApiResponse<ShippingDocumentDto>.Fail(result.ErrorCode ?? "SHIPPING_NOT_FOUND", result.Message ?? "Shipping document was not found."));
        }

        return Ok(ApiResponse<ShippingDocumentDto>.Ok(result.Value));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ShippingDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ShippingDocumentDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ShippingDocumentDto>>> Create(CreateShippingRequest request, CancellationToken cancellationToken)
    {
        var result = await _shippingService.CreateAsync(request, _userContextService.UserName, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<ShippingDocumentDto>.Fail(result.ErrorCode ?? "SHIPPING_ERROR", result.Message ?? "Shipping document could not be created."));
        }

        return Ok(ApiResponse<ShippingDocumentDto>.Ok(result.Value, result.Message));
    }

    [HttpPost("confirm")]
    [ProducesResponseType(typeof(ApiResponse<ShippingDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ShippingDocumentDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ShippingDocumentDto>>> Confirm(ConfirmShippingRequest request, CancellationToken cancellationToken)
    {
        var result = await _shippingService.ConfirmAsync(request, _userContextService.UserName, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<ShippingDocumentDto>.Fail(result.ErrorCode ?? "SHIPPING_ERROR", result.Message ?? "Shipping document could not be confirmed."));
        }

        return Ok(ApiResponse<ShippingDocumentDto>.Ok(result.Value, result.Message));
    }

    [HttpPost("create-delivery")]
    [ProducesResponseType(typeof(ApiResponse<ShippingDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ShippingDocumentDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ShippingDocumentDto>>> CreateDelivery(CreateSapDeliveryRequest request, CancellationToken cancellationToken)
    {
        var result = await _shippingService.CreateSapDeliveryAsync(request, _userContextService.UserName, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<ShippingDocumentDto>.Fail(result.ErrorCode ?? "SAP_DELIVERY_ERROR", result.Message ?? "SAP delivery could not be created."));
        }

        return Ok(ApiResponse<ShippingDocumentDto>.Ok(result.Value, result.Message));
    }

    [HttpPost("cancel")]
    [ProducesResponseType(typeof(ApiResponse<ShippingDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ShippingDocumentDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ShippingDocumentDto>>> Cancel(CancelShippingRequest request, CancellationToken cancellationToken)
    {
        var result = await _shippingService.CancelAsync(request, _userContextService.UserName, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<ShippingDocumentDto>.Fail(result.ErrorCode ?? "SHIPPING_ERROR", result.Message ?? "Shipping document could not be cancelled."));
        }

        return Ok(ApiResponse<ShippingDocumentDto>.Ok(result.Value, result.Message));
    }
}
