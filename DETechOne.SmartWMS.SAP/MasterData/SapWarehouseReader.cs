using DETechOne.SmartWMS.Application.SAP;
using DETechOne.SmartWMS.Contracts.Dtos.SAP;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.SAP.MasterData;

public sealed class SapWarehouseReader : ISapWarehouseReader
{
    public Task<Result<SapWarehouseDto>> GetByWarehouseCodeAsync(string warehouseCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(warehouseCode))
        {
            return Task.FromResult(Result<SapWarehouseDto>.Fail("SAP_WAREHOUSE_CODE_REQUIRED", "WarehouseCode is required."));
        }

        return Task.FromResult(Result<SapWarehouseDto>.Fail(
            "SAP_ADAPTER_NOT_ENABLED",
            "Warehouse reader contract is ready. Implement Service Layer or DI API mapping when SAP connectivity is available."));
    }
}
