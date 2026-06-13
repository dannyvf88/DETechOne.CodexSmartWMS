using DETechOne.SmartWMS.Contracts.Dtos.Stabilization;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.Stabilization;

public interface IMvpReadinessService
{
    Task<Result<MvpReadinessDto>> CheckAsync(CancellationToken cancellationToken = default);
}
