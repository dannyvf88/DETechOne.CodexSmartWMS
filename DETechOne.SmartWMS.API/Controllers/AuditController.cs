using DETechOne.SmartWMS.Application.Audit;
using DETechOne.SmartWMS.Application.Security;
using DETechOne.SmartWMS.Contracts.Dtos.Audit;
using DETechOne.SmartWMS.Contracts.Requests.Audit;
using DETechOne.SmartWMS.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DETechOne.SmartWMS.API.Controllers;

[ApiController]
[Route("api/audit")]
public sealed class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;
    private readonly IUserContextService _userContextService;

    public AuditController(IAuditService auditService, IUserContextService userContextService)
    {
        _auditService = auditService;
        _userContextService = userContextService;
    }

    [HttpPost("events")]
    [ProducesResponseType(typeof(ApiResponse<AuditLogEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuditLogEntryDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuditLogEntryDto>>> WriteEvent(
        CreateAuditLogRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _auditService
            .WriteAsync(request, ResolveUserName(), cancellationToken)
            .ConfigureAwait(false);

        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<AuditLogEntryDto>.Fail(
                result.ErrorCode ?? "AUDIT_WRITE_ERROR",
                result.Message ?? "Audit event could not be registered."));
        }

        return Ok(ApiResponse<AuditLogEntryDto>.Ok(result.Value, result.Message));
    }

    [HttpGet("events")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AuditLogEntryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<AuditLogEntryDto>>>> SearchEvents(
        [FromQuery] AuditLogQueryRequest query,
        CancellationToken cancellationToken)
    {
        var result = await _auditService.SearchAsync(query, cancellationToken).ConfigureAwait(false);

        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<PagedResult<AuditLogEntryDto>>.Fail(
                result.ErrorCode ?? "AUDIT_SEARCH_ERROR",
                result.Message ?? "Audit events could not be retrieved."));
        }

        return Ok(ApiResponse<PagedResult<AuditLogEntryDto>>.Ok(result.Value, result.Message));
    }

    private string ResolveUserName()
    {
        return string.IsNullOrWhiteSpace(_userContextService.UserName)
            ? "SmartWMS.API"
            : _userContextService.UserName;
    }
}
