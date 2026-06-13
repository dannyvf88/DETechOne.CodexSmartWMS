using DETechOne.SmartWMS.Contracts.Dtos.SAP;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.SAP;

public interface ISapSalesOrderReader
{
    Task<Result<SapSalesOrderDto>> GetByDocEntryAsync(int docEntry, CancellationToken cancellationToken = default);
}
