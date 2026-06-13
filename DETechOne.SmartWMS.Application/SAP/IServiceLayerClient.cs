using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.SAP;

public interface IServiceLayerClient
{
    Task<Result<string>> GetAsync(string relativeUrl, CancellationToken cancellationToken = default);
    Task<Result<string>> PostAsync(string relativeUrl, object payload, CancellationToken cancellationToken = default);
}
