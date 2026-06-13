using System.Security.Claims;
using DETechOne.SmartWMS.Application.Security;

namespace DETechOne.SmartWMS.API.Security;

public sealed class HttpUserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpUserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var userId) ? userId : null;
        }
    }

    public string? UserName => _httpContextAccessor.HttpContext?.User.Identity?.Name;

    public string? CompanyCode => _httpContextAccessor.HttpContext?.User.FindFirstValue("company_code");

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;
}
