using DETechOne.SmartWMS.Contracts.Dtos.Metadata;

namespace DETechOne.SmartWMS.Application.Metadata;

public interface ISapMetadataService
{
    Task<bool> ExistsAsync(MetadataObjectDefinitionDto definition, string companyCode, CancellationToken cancellationToken = default);

    Task CreateOrUpdateAsync(MetadataObjectDefinitionDto definition, string companyCode, CancellationToken cancellationToken = default);
}
