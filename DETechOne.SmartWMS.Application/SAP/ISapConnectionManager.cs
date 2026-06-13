using DETechOne.SmartWMS.Contracts.Dtos.SAP;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.SAP;

public interface ISapConnectionManager
{
    Task<Result<SapConnectionStatusDto>> GetStatusAsync(CancellationToken cancellationToken = default);
}
