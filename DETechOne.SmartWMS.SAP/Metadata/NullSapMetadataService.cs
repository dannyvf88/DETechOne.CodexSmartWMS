using DETechOne.SmartWMS.Application.Metadata;
using DETechOne.SmartWMS.Contracts.Dtos.Metadata;

namespace DETechOne.SmartWMS.SAP.Metadata;

public sealed class NullSapMetadataService : ISapMetadataService
{
    public Task<bool> ExistsAsync(MetadataObjectDefinitionDto definition, string companyCode, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentException.ThrowIfNullOrWhiteSpace(companyCode);

        return Task.FromResult(false);
    }

    public Task CreateOrUpdateAsync(MetadataObjectDefinitionDto definition, string companyCode, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentException.ThrowIfNullOrWhiteSpace(companyCode);

        return Task.CompletedTask;
    }
}
