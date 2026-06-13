namespace DETechOne.SmartWMS.Contracts.Dtos.Schema;

public sealed record SchemaMenuDefinitionDto(
    string Code,
    string Name,
    string ParentCode,
    int Position,
    string? LinkedObjectCode = null,
    string? PermissionCode = null);
