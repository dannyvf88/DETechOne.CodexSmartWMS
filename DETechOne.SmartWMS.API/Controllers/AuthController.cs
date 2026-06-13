using DETechOne.SmartWMS.Application.Security;
using DETechOne.SmartWMS.Contracts.Requests.Auth;
using DETechOne.SmartWMS.Contracts.Responses;
using DETechOne.SmartWMS.Contracts.Responses.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DETechOne.SmartWMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserContextService _userContextService;

    public AuthController(IAuthService authService, IUserContextService userContextService)
    {
        _authService = authService;
        _userContextService = userContextService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);

        if (!result.Success || result.Value is null)
        {
            return Unauthorized(ApiResponse<LoginResponse>.Fail(result.ErrorCode ?? "AUTH_ERROR", result.Message ?? "No autorizado."));
        }

        return Ok(ApiResponse<LoginResponse>.Ok(result.Value, result.Message));
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<object>> Me()
    {
        var data = new
        {
            _userContextService.UserId,
            _userContextService.UserName,
            _userContextService.CompanyCode,
            _userContextService.IsAuthenticated
        };

        return Ok(ApiResponse<object>.Ok(data));
    }
}
