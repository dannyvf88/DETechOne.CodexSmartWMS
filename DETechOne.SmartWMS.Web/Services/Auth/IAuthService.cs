using DETechOne.SmartWMS.Web.Models.Auth;

namespace DETechOne.SmartWMS.Web.Services.Auth;

public interface IAuthService
{
    Task<bool> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task LogoutAsync(CancellationToken cancellationToken = default);
}
