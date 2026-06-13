namespace DETechOne.SmartWMS.Contracts.Dtos.Schema;

public sealed record SchemaUdoDefinitionDto(
    string Code,
    string Name,
    string MainTableCode,
    string ObjectType,
    bool CanCancel,
    bool CanClose,
    bool CanCreateDefaultForm,
    bool CanDelete,
    bool CanFind,
    bool ManageSeries,
    IReadOnlyList<string> FindColumns,
    IReadOnlyList<SchemaUdoChildTableDefinitionDto> ChildTables);
