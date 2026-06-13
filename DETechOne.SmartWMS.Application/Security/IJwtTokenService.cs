using DETechOne.SmartWMS.Contracts.Dtos.Auth;

namespace DETechOne.SmartWMS.Application.Security;

public interface IJwtTokenService
{
    string CreateToken(AuthUserDto user, DateTime expiresAtUtc);
}
