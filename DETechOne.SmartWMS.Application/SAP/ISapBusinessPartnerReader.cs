using DETechOne.SmartWMS.Contracts.Dtos.SAP;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.SAP;

public interface ISapBusinessPartnerReader
{
    Task<Result<SapBusinessPartnerDto>> GetByCardCodeAsync(string cardCode, CancellationToken cancellationToken = default);
}
