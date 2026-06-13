using DETechOne.SmartWMS.Application.SAP;
using DETechOne.SmartWMS.Application.Security;
using DETechOne.SmartWMS.Contracts.Dtos.SAP;
using DETechOne.SmartWMS.Contracts.Requests.SAP;
using DETechOne.SmartWMS.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DETechOne.SmartWMS.API.Controllers;

[ApiController]
[Authorize]
[Route("api/sap")]
public sealed class SapController : ControllerBase
{
    private readonly ISapConnectionManager _connectionManager;
    private readonly ISapSessionManager _sessionManager;
    private readonly ISapBusinessPartnerReader _businessPartnerReader;
    private readonly ISapSalesOrderReader _salesOrderReader;
    private readonly ISapItemMasterReader _itemMasterReader;
    private readonly ISapWarehouseReader _warehouseReader;
    private readonly ISapInventoryTransferService _inventoryTransferService;
    private readonly IUserContextService _userContextService;

    public SapController(
        ISapConnectionManager connectionManager,
        ISapSessionManager sessionManager,
        ISapBusinessPartnerReader businessPartnerReader,
        ISapSalesOrderReader salesOrderReader,
        ISapItemMasterReader itemMasterReader,
        ISapWarehouseReader warehouseReader,
        ISapInventoryTransferService inventoryTransferService,
        IUserContextService userContextService)
    {
        _connectionManager = connectionManager;
        _sessionManager = sessionManager;
        _businessPartnerReader = businessPartnerReader;
        _salesOrderReader = salesOrderReader;
        _itemMasterReader = itemMasterReader;
        _warehouseReader = warehouseReader;
        _inventoryTransferService = inventoryTransferService;
        _userContextService = userContextService;
    }

    [HttpGet("status")]
    [ProducesResponseType(typeof(ApiResponse<SapConnectionStatusDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<SapConnectionStatusDto>>> GetStatus(CancellationToken cancellationToken)
    {
        var result = await _connectionManager.GetStatusAsync(cancellationToken).ConfigureAwait(false);
        return Ok(ApiResponse<SapConnectionStatusDto>.Ok(result.Value!, result.Message));
    }

    [HttpPost("service-layer/login")]
    [ProducesResponseType(typeof(ApiResponse<SapSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SapSessionDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<SapSessionDto>>> LoginServiceLayer(CancellationToken cancellationToken)
    {
        var result = await _sessionManager.LoginAsync(cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<SapSessionDto>.Fail(result.ErrorCode ?? "SAP_LOGIN_ERROR", result.Message ?? "SAP Service Layer login failed."));
        }

        return Ok(ApiResponse<SapSessionDto>.Ok(result.Value, result.Message));
    }

    [HttpPost("service-layer/logout")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<string>>> LogoutServiceLayer(CancellationToken cancellationToken)
    {
        var result = await _sessionManager.LogoutAsync(cancellationToken).ConfigureAwait(false);
        return Ok(ApiResponse<string>.Ok("OK", result.Message));
    }

    [HttpGet("business-partners/{cardCode}")]
    [ProducesResponseType(typeof(ApiResponse<SapBusinessPartnerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SapBusinessPartnerDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<SapBusinessPartnerDto>>> GetBusinessPartner(string cardCode, CancellationToken cancellationToken)
    {
        var result = await _businessPartnerReader.GetByCardCodeAsync(cardCode, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<SapBusinessPartnerDto>.Fail(result.ErrorCode ?? "SAP_BP_ERROR", result.Message ?? "Business Partner could not be read from SAP."));
        }

        return Ok(ApiResponse<SapBusinessPartnerDto>.Ok(result.Value));
    }

    [HttpGet("sales-orders/{docEntry:int}")]
    [ProducesResponseType(typeof(ApiResponse<SapSalesOrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SapSalesOrderDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<SapSalesOrderDto>>> GetSalesOrder(int docEntry, CancellationToken cancellationToken)
    {
        var result = await _salesOrderReader.GetByDocEntryAsync(docEntry, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<SapSalesOrderDto>.Fail(result.ErrorCode ?? "SAP_ORDER_ERROR", result.Message ?? "Sales Order could not be read from SAP."));
        }

        return Ok(ApiResponse<SapSalesOrderDto>.Ok(result.Value));
    }

    [HttpGet("items/{itemCode}")]
    [ProducesResponseType(typeof(ApiResponse<SapItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SapItemDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<SapItemDto>>> GetItem(string itemCode, CancellationToken cancellationToken)
    {
        var result = await _itemMasterReader.GetByItemCodeAsync(itemCode, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<SapItemDto>.Fail(result.ErrorCode ?? "SAP_ITEM_ERROR", result.Message ?? "Item could not be read from SAP."));
        }

        return Ok(ApiResponse<SapItemDto>.Ok(result.Value));
    }

    [HttpGet("warehouses/{warehouseCode}")]
    [ProducesResponseType(typeof(ApiResponse<SapWarehouseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SapWarehouseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<SapWarehouseDto>>> GetWarehouse(string warehouseCode, CancellationToken cancellationToken)
    {
        var result = await _warehouseReader.GetByWarehouseCodeAsync(warehouseCode, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<SapWarehouseDto>.Fail(result.ErrorCode ?? "SAP_WAREHOUSE_ERROR", result.Message ?? "Warehouse could not be read from SAP."));
        }

        return Ok(ApiResponse<SapWarehouseDto>.Ok(result.Value));
    }

    [HttpPost("inventory-transfers")]
    [ProducesResponseType(typeof(ApiResponse<SapInventoryTransferResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SapInventoryTransferResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<SapInventoryTransferResultDto>>> CreateInventoryTransfer(
        CreateSapInventoryTransferRequest request,
        CancellationToken cancellationToken)
    {
        var userName = _userContextService.UserName;

        if (string.IsNullOrWhiteSpace(userName))
        {
            return Unauthorized();
        }

        var result = await _inventoryTransferService
            .CreateTransferAsync(request, userName, cancellationToken)
            .ConfigureAwait(false);

        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<SapInventoryTransferResultDto>.Fail(
                result.ErrorCode ?? "SAP_TRANSFER_ERROR",
                result.Message ?? "Inventory Transfer could not be created in SAP."));
        }

        return Ok(ApiResponse<SapInventoryTransferResultDto>.Ok(result.Value, result.Message));
    }
}
