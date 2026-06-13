using DETechOne.SmartWMS.Application.Stabilization;
using DETechOne.SmartWMS.Contracts.Dtos.Stabilization;
using DETechOne.SmartWMS.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DETechOne.SmartWMS.API.Controllers;

[ApiController]
[Route("api/readiness")]
public sealed class ReadinessController : ControllerBase
{
    private readonly IMvpReadinessService _mvpReadinessService;

    public ReadinessController(IMvpReadinessService mvpReadinessService)
    {
        _mvpReadinessService = mvpReadinessService;
    }

    [HttpGet("mvp")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<MvpReadinessDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<MvpReadinessDto>), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<ApiResponse<MvpReadinessDto>>> GetMvpReadiness(CancellationToken cancellationToken)
    {
        var result = await _mvpReadinessService.CheckAsync(cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Value is null)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ApiResponse<MvpReadinessDto>.Fail(
                result.ErrorCode ?? "MVP_READINESS_ERROR",
                result.Message ?? "SmartWMS backend MVP readiness check failed."));
        }

        if (string.Equals(result.Value.Status, "NOT_READY", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ApiResponse<MvpReadinessDto>.Ok(result.Value, result.Message));
        }

        return Ok(ApiResponse<MvpReadinessDto>.Ok(result.Value, result.Message));
    }
}
