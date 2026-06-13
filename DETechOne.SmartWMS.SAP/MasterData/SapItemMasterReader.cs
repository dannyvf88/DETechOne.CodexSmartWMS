using DETechOne.SmartWMS.Application.SAP;
using DETechOne.SmartWMS.Contracts.Dtos.SAP;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.SAP.MasterData;

public sealed class SapItemMasterReader : ISapItemMasterReader
{
    public Task<Result<SapItemDto>> GetByItemCodeAsync(string itemCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(itemCode))
        {
            return Task.FromResult(Result<SapItemDto>.Fail("SAP_ITEM_CODE_REQUIRED", "ItemCode is required."));
        }

        return Task.FromResult(Result<SapItemDto>.Fail(
            "SAP_ADAPTER_NOT_ENABLED",
            "Item Master reader contract is ready. Implement Service Layer or DI API mapping when SAP connectivity is available."));
    }
}
