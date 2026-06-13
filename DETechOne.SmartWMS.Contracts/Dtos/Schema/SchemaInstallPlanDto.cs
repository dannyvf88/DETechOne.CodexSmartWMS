namespace DETechOne.SmartWMS.Contracts.Dtos.Schema;

public sealed record SchemaInstallPlanDto(
    string Version,
    IReadOnlyList<SchemaTableDefinitionDto> Tables,
    IReadOnlyList<SchemaUdoDefinitionDto> Udos,
    IReadOnlyList<SchemaPermissionDefinitionDto> Permissions,
    IReadOnlyList<SchemaMenuDefinitionDto> Menus,
    IReadOnlyList<SchemaSeedDataDefinitionDto> SeedData);
