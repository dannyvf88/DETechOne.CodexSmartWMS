using DETechOne.SmartWMS.Contracts.Dtos.SAP;
using DETechOne.SmartWMS.Contracts.Requests.SAP;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.SAP;

public interface ISapInventoryTransferService
{
    Task<Result<SapInventoryTransferResultDto>> CreateTransferAsync(CreateSapInventoryTransferRequest request, string userName, CancellationToken cancellationToken = default);
}
