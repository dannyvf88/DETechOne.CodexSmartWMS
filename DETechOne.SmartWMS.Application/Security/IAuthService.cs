using DETechOne.SmartWMS.Contracts.Requests.Auth;
using DETechOne.SmartWMS.Contracts.Responses.Auth;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.Security;

public interface IAuthService
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
