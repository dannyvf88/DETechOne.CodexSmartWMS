using DETechOne.SmartWMS.Contracts.Dtos.SAP;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.SAP;

public interface ISapWarehouseReader
{
    Task<Result<SapWarehouseDto>> GetByWarehouseCodeAsync(string warehouseCode, CancellationToken cancellationToken = default);
}
