using DETechOne.SmartWMS.Contracts.Dtos.Metadata;

namespace DETechOne.SmartWMS.Application.Metadata;

public interface IMetadataDefinitionProvider
{
    IReadOnlyList<MetadataObjectDefinitionDto> GetDefinitions();
}
