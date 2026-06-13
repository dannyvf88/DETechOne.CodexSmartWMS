using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Contracts.Dtos.Metadata;

public sealed record MetadataObjectDefinitionDto(
    string Code,
    string Name,
    MetadataObjectType ObjectType,
    string Description,
    IReadOnlyList<MetadataFieldDefinitionDto> Fields);
