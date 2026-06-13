namespace DETechOne.SmartWMS.Application.Security;

public interface IUserContextService
{
    Guid? UserId { get; }
    string? UserName { get; }
    string? CompanyCode { get; }
    bool IsAuthenticated { get; }
}
