using DETechOne.SmartWMS.Application.SAP;
using DETechOne.SmartWMS.Contracts.Dtos.SAP;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.SAP.MasterData;

public sealed class SapBusinessPartnerReader : ISapBusinessPartnerReader
{
    public Task<Result<SapBusinessPartnerDto>> GetByCardCodeAsync(string cardCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cardCode))
        {
            return Task.FromResult(Result<SapBusinessPartnerDto>.Fail("SAP_CARD_CODE_REQUIRED", "CardCode is required."));
        }

        return Task.FromResult(Result<SapBusinessPartnerDto>.Fail(
            "SAP_ADAPTER_NOT_ENABLED",
            "Business Partner reader contract is ready. Implement Service Layer or DI API mapping when SAP connectivity is available."));
    }
}
