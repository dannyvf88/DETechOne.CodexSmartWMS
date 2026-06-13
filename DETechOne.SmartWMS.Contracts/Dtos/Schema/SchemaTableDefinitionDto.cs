namespace DETechOne.SmartWMS.Contracts.Dtos.Schema;

public sealed record SchemaTableDefinitionDto(
    string Code,
    string Name,
    string SapTableType,
    string Description,
    IReadOnlyList<SchemaFieldDefinitionDto> Fields);
