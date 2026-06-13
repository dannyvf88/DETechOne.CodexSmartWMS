using DETechOne.SmartWMS.Contracts.Dtos;
using DETechOne.SmartWMS.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DETechOne.SmartWMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<HealthDto>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<HealthDto>> Get()
    {
        var dto = new HealthDto("DETechOne.SmartWMS.API", "OK", DateTime.UtcNow, "2.0.0");
        return Ok(ApiResponse<HealthDto>.Ok(dto));
    }
}
