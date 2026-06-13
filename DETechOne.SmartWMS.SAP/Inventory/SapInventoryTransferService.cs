using DETechOne.SmartWMS.Application.SAP;
using DETechOne.SmartWMS.Contracts.Dtos.SAP;
using DETechOne.SmartWMS.Contracts.Requests.SAP;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.SAP.Inventory;

public sealed class SapInventoryTransferService : ISapInventoryTransferService
{
    public Task<Result<SapInventoryTransferResultDto>> CreateTransferAsync(CreateSapInventoryTransferRequest request, string userName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.FromWarehouseCode) || string.IsNullOrWhiteSpace(request.ToWarehouseCode))
        {
            return Task.FromResult(Result<SapInventoryTransferResultDto>.Fail("SAP_TRANSFER_WAREHOUSE_REQUIRED", "FromWarehouseCode and ToWarehouseCode are required."));
        }

        if (request.Lines.Count == 0)
        {
            return Task.FromResult(Result<SapInventoryTransferResultDto>.Fail("SAP_TRANSFER_LINES_REQUIRED", "At least one transfer line is required."));
        }

        return Task.FromResult(Result<SapInventoryTransferResultDto>.Fail(
            "SAP_ADAPTER_NOT_ENABLED",
            "Inventory Transfer service contract is ready. Implement DI API document creation when SAP SDK is available."));
    }
}
