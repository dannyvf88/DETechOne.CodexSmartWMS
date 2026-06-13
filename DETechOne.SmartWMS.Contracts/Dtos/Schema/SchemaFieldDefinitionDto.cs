namespace DETechOne.SmartWMS.Contracts.Dtos.Schema;

public sealed record SchemaFieldDefinitionDto(
    string Name,
    string Description,
    string SapType,
    int Size = 0,
    int EditSize = 0,
    bool Mandatory = false,
    string? DefaultValue = null,
    string? LinkedTable = null,
    IReadOnlyDictionary<string, string>? ValidValues = null);
