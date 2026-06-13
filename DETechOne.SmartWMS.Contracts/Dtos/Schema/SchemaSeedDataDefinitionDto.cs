namespace DETechOne.SmartWMS.Contracts.Dtos.Schema;

public sealed record SchemaSeedDataDefinitionDto(
    string TableCode,
    string Code,
    string Name,
    IReadOnlyDictionary<string, string> Values);
