using DETechOne.SmartWMS.Contracts.Dtos.SAP;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.SAP;

public interface ISapSessionManager
{
    Task<Result<SapSessionDto>> LoginAsync(CancellationToken cancellationToken = default);
    Task<Result> LogoutAsync(CancellationToken cancellationToken = default);
}
