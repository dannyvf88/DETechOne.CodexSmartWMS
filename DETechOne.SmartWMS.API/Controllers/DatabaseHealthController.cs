using DETechOne.SmartWMS.Application.Persistence;
using DETechOne.SmartWMS.Contracts.Dtos.Database;
using DETechOne.SmartWMS.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DETechOne.SmartWMS.API.Controllers;

[ApiController]
[Route("api/health/database")]
public sealed class DatabaseHealthController : ControllerBase
{
    private readonly IDatabaseHealthCheck _databaseHealthCheck;

    public DatabaseHealthController(IDatabaseHealthCheck databaseHealthCheck)
    {
        _databaseHealthCheck = databaseHealthCheck;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<DatabaseHealthDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<DatabaseHealthDto>), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<ApiResponse<DatabaseHealthDto>>> Get(CancellationToken cancellationToken)
    {
        DatabaseHealthResult result = await _databaseHealthCheck.CheckAsync(cancellationToken).ConfigureAwait(false);

        var dto = new DatabaseHealthDto(
            result.Success,
            result.Provider,
            result.DatabaseType,
            result.Message,
            result.CheckedAtUtc,
            result.ElapsedMilliseconds);

        if (!result.Success)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ApiResponse<DatabaseHealthDto>.Fail("DATABASE_UNAVAILABLE", result.Message));
        }

        return Ok(ApiResponse<DatabaseHealthDto>.Ok(dto));
    }
}
