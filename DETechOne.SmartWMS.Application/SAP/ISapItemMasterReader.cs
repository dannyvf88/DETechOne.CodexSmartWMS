using DETechOne.SmartWMS.Contracts.Dtos.SAP;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.SAP;

public interface ISapItemMasterReader
{
    Task<Result<SapItemDto>> GetByItemCodeAsync(string itemCode, CancellationToken cancellationToken = default);
}
