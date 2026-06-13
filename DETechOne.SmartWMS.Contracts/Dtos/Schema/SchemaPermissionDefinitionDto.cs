namespace DETechOne.SmartWMS.Contracts.Dtos.Schema;

public sealed record SchemaPermissionDefinitionDto(
    string Code,
    string Name,
    string Description,
    string Category,
    string DefaultAuthorization);
