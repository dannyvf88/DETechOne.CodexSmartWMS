namespace DETechOne.SmartWMS.Contracts.Dtos.Metadata;

public sealed record MetadataFieldDefinitionDto(
    string Name,
    string Description,
    string DataType,
    int Size,
    bool Mandatory = false,
    string? ValidValues = null);
