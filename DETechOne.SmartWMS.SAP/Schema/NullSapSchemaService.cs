using DETechOne.SmartWMS.Application.Schema;
using DETechOne.SmartWMS.Contracts.Dtos.Schema;

namespace DETechOne.SmartWMS.SAP.Schema;

public sealed class NullSapSchemaService : ISapSchemaService
{
    private static readonly HashSet<string> InstalledUserTables = new(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> InstalledUserFields = new(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> InstalledUdos = new(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> InstalledPermissions = new(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> InstalledMenus = new(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> InstalledSeedData = new(StringComparer.OrdinalIgnoreCase);

    public Task<bool> UserTableExistsAsync(string companyCode, string tableCode, CancellationToken cancellationToken = default)
    {
        Validate(companyCode, tableCode);
        return Task.FromResult(InstalledUserTables.Contains(Key(companyCode, tableCode)));
    }

    public Task CreateOrUpdateUserTableAsync(string companyCode, SchemaTableDefinitionDto table, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(table);
        Validate(companyCode, table.Code);
        InstalledUserTables.Add(Key(companyCode, table.Code));
        return Task.CompletedTask;
    }

    public Task<bool> UserFieldExistsAsync(string companyCode, string tableCode, string fieldName, CancellationToken cancellationToken = default)
    {
        Validate(companyCode, tableCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldName);
        return Task.FromResult(InstalledUserFields.Contains(Key(companyCode, tableCode, fieldName)));
    }

    public Task CreateOrUpdateUserFieldAsync(string companyCode, string tableCode, SchemaFieldDefinitionDto field, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(field);
        Validate(companyCode, tableCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(field.Name);
        InstalledUserFields.Add(Key(companyCode, tableCode, field.Name));
        return Task.CompletedTask;
    }

    public Task<bool> UdoExistsAsync(string companyCode, string udoCode, CancellationToken cancellationToken = default)
    {
        Validate(companyCode, udoCode);
        return Task.FromResult(InstalledUdos.Contains(Key(companyCode, udoCode)));
    }

    public Task CreateOrUpdateUdoAsync(string companyCode, SchemaUdoDefinitionDto udo, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(udo);
        Validate(companyCode, udo.Code);
        InstalledUdos.Add(Key(companyCode, udo.Code));
        return Task.CompletedTask;
    }

    public Task<bool> PermissionExistsAsync(string companyCode, string permissionCode, CancellationToken cancellationToken = default)
    {
        Validate(companyCode, permissionCode);
        return Task.FromResult(InstalledPermissions.Contains(Key(companyCode, permissionCode)));
    }

    public Task CreateOrUpdatePermissionAsync(string companyCode, SchemaPermissionDefinitionDto permission, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(permission);
        Validate(companyCode, permission.Code);
        InstalledPermissions.Add(Key(companyCode, permission.Code));
        return Task.CompletedTask;
    }

    public Task<bool> MenuExistsAsync(string companyCode, string menuCode, CancellationToken cancellationToken = default)
    {
        Validate(companyCode, menuCode);
        return Task.FromResult(InstalledMenus.Contains(Key(companyCode, menuCode)));
    }

    public Task CreateOrUpdateMenuAsync(string companyCode, SchemaMenuDefinitionDto menu, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(menu);
        Validate(companyCode, menu.Code);
        InstalledMenus.Add(Key(companyCode, menu.Code));
        return Task.CompletedTask;
    }

    public Task<bool> SeedDataExistsAsync(string companyCode, string tableCode, string code, CancellationToken cancellationToken = default)
    {
        Validate(companyCode, tableCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        return Task.FromResult(InstalledSeedData.Contains(Key(companyCode, tableCode, code)));
    }

    public Task CreateOrUpdateSeedDataAsync(string companyCode, SchemaSeedDataDefinitionDto seedData, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(seedData);
        Validate(companyCode, seedData.TableCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(seedData.Code);
        InstalledSeedData.Add(Key(companyCode, seedData.TableCode, seedData.Code));
        return Task.CompletedTask;
    }

    private static void Validate(string companyCode, string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(companyCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
    }

    private static string Key(params string[] parts) => string.Join("::", parts);
}
