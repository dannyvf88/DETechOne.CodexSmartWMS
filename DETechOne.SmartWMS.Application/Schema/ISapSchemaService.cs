using DETechOne.SmartWMS.Contracts.Dtos.Schema;

namespace DETechOne.SmartWMS.Application.Schema;

public interface ISapSchemaService
{
    Task<bool> UserTableExistsAsync(string companyCode, string tableCode, CancellationToken cancellationToken = default);
    Task CreateOrUpdateUserTableAsync(string companyCode, SchemaTableDefinitionDto table, CancellationToken cancellationToken = default);

    Task<bool> UserFieldExistsAsync(string companyCode, string tableCode, string fieldName, CancellationToken cancellationToken = default);
    Task CreateOrUpdateUserFieldAsync(string companyCode, string tableCode, SchemaFieldDefinitionDto field, CancellationToken cancellationToken = default);

    Task<bool> UdoExistsAsync(string companyCode, string udoCode, CancellationToken cancellationToken = default);
    Task CreateOrUpdateUdoAsync(string companyCode, SchemaUdoDefinitionDto udo, CancellationToken cancellationToken = default);

    Task<bool> PermissionExistsAsync(string companyCode, string permissionCode, CancellationToken cancellationToken = default);
    Task CreateOrUpdatePermissionAsync(string companyCode, SchemaPermissionDefinitionDto permission, CancellationToken cancellationToken = default);

    Task<bool> MenuExistsAsync(string companyCode, string menuCode, CancellationToken cancellationToken = default);
    Task CreateOrUpdateMenuAsync(string companyCode, SchemaMenuDefinitionDto menu, CancellationToken cancellationToken = default);

    Task<bool> SeedDataExistsAsync(string companyCode, string tableCode, string code, CancellationToken cancellationToken = default);
    Task CreateOrUpdateSeedDataAsync(string companyCode, SchemaSeedDataDefinitionDto seedData, CancellationToken cancellationToken = default);
}
