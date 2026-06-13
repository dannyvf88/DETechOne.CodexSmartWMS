using DETechOne.SmartWMS.Application.Common;
using DETechOne.SmartWMS.Application.Schema;
using DETechOne.SmartWMS.Contracts.Dtos.Schema;
using DETechOne.SmartWMS.Contracts.Requests.Schema;

namespace DETechOne.SmartWMS.Installer.Services.Schema;

public sealed class SmartWmsDatabaseSchemaInstaller : IDatabaseSchemaInstaller
{
    private readonly IDatabaseSchemaDefinitionProvider _definitionProvider;
    private readonly ISapSchemaService _sapSchemaService;
    private readonly IClock _clock;

    public SmartWmsDatabaseSchemaInstaller(
        IDatabaseSchemaDefinitionProvider definitionProvider,
        ISapSchemaService sapSchemaService,
        IClock clock)
    {
        _definitionProvider = definitionProvider;
        _sapSchemaService = sapSchemaService;
        _clock = clock;
    }

    public async Task<SchemaInstallResultDto> InstallAsync(RunSchemaInstallRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.CompanyCode);

        DateTimeOffset startedAt = new(DateTime.SpecifyKind(_clock.UtcNow, DateTimeKind.Utc));
        SchemaInstallPlanDto plan = _definitionProvider.GetPlan();
        var steps = new List<SchemaInstallStepResultDto>();

        if (request.InstallTables)
        {
            foreach (SchemaTableDefinitionDto table in plan.Tables)
            {
                bool shouldContinue = await ExecuteStepAsync(
                    steps,
                    request,
                    table.Code,
                    table.Name,
                    "UDT",
                    async () => await InstallTableAsync(request, table, cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);

                if (!shouldContinue)
                {
                    return BuildResult(request, startedAt, steps);
                }
            }
        }

        if (request.InstallUdos)
        {
            foreach (SchemaUdoDefinitionDto udo in plan.Udos)
            {
                bool shouldContinue = await ExecuteStepAsync(
                    steps,
                    request,
                    udo.Code,
                    udo.Name,
                    "UDO",
                    async () => await InstallUdoAsync(request, udo, cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);

                if (!shouldContinue)
                {
                    return BuildResult(request, startedAt, steps);
                }
            }
        }

        if (request.InstallPermissions)
        {
            foreach (SchemaPermissionDefinitionDto permission in plan.Permissions)
            {
                bool shouldContinue = await ExecuteStepAsync(
                    steps,
                    request,
                    permission.Code,
                    permission.Name,
                    "PERMISSION",
                    async () => await InstallPermissionAsync(request, permission, cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);

                if (!shouldContinue)
                {
                    return BuildResult(request, startedAt, steps);
                }
            }
        }

        if (request.InstallMenus)
        {
            foreach (SchemaMenuDefinitionDto menu in plan.Menus)
            {
                bool shouldContinue = await ExecuteStepAsync(
                    steps,
                    request,
                    menu.Code,
                    menu.Name,
                    "MENU",
                    async () => await InstallMenuAsync(request, menu, cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);

                if (!shouldContinue)
                {
                    return BuildResult(request, startedAt, steps);
                }
            }
        }

        if (request.InstallSeedData)
        {
            foreach (SchemaSeedDataDefinitionDto seedData in plan.SeedData)
            {
                bool shouldContinue = await ExecuteStepAsync(
                    steps,
                    request,
                    $"{seedData.TableCode}:{seedData.Code}",
                    seedData.Name,
                    "SEED_DATA",
                    async () => await InstallSeedDataAsync(request, seedData, cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);

                if (!shouldContinue)
                {
                    return BuildResult(request, startedAt, steps);
                }
            }
        }

        return BuildResult(request, startedAt, steps);
    }

    private async Task<SchemaStepOutcome> InstallTableAsync(RunSchemaInstallRequest request, SchemaTableDefinitionDto table, CancellationToken cancellationToken)
    {
        if (request.DryRun)
        {
            return SchemaStepOutcome.AsSkipped($"DryRun activo. Se validaria/crearia la UDT {table.Code} con {table.Fields.Count} campos.");
        }

        bool tableExists = await _sapSchemaService.UserTableExistsAsync(request.CompanyCode, table.Code, cancellationToken).ConfigureAwait(false);
        await _sapSchemaService.CreateOrUpdateUserTableAsync(request.CompanyCode, table, cancellationToken).ConfigureAwait(false);

        foreach (SchemaFieldDefinitionDto field in table.Fields)
        {
            await _sapSchemaService.CreateOrUpdateUserFieldAsync(request.CompanyCode, table.Code, field, cancellationToken).ConfigureAwait(false);
        }

        return tableExists
            ? SchemaStepOutcome.AsUpdated($"UDT {table.Code} actualizada con {table.Fields.Count} campos.")
            : SchemaStepOutcome.AsCreated($"UDT {table.Code} creada con {table.Fields.Count} campos.");
    }

    private async Task<SchemaStepOutcome> InstallUdoAsync(RunSchemaInstallRequest request, SchemaUdoDefinitionDto udo, CancellationToken cancellationToken)
    {
        if (request.DryRun)
        {
            return SchemaStepOutcome.AsSkipped($"DryRun activo. Se validaria/crearia el UDO {udo.Code}.");
        }

        bool exists = await _sapSchemaService.UdoExistsAsync(request.CompanyCode, udo.Code, cancellationToken).ConfigureAwait(false);
        await _sapSchemaService.CreateOrUpdateUdoAsync(request.CompanyCode, udo, cancellationToken).ConfigureAwait(false);

        return exists
            ? SchemaStepOutcome.AsUpdated($"UDO {udo.Code} actualizado.")
            : SchemaStepOutcome.AsCreated($"UDO {udo.Code} creado.");
    }

    private async Task<SchemaStepOutcome> InstallPermissionAsync(RunSchemaInstallRequest request, SchemaPermissionDefinitionDto permission, CancellationToken cancellationToken)
    {
        if (request.DryRun)
        {
            return SchemaStepOutcome.AsSkipped($"DryRun activo. Se validaria/crearia el permiso {permission.Code}.");
        }

        bool exists = await _sapSchemaService.PermissionExistsAsync(request.CompanyCode, permission.Code, cancellationToken).ConfigureAwait(false);
        await _sapSchemaService.CreateOrUpdatePermissionAsync(request.CompanyCode, permission, cancellationToken).ConfigureAwait(false);

        return exists
            ? SchemaStepOutcome.AsUpdated($"Permiso {permission.Code} actualizado.")
            : SchemaStepOutcome.AsCreated($"Permiso {permission.Code} creado.");
    }

    private async Task<SchemaStepOutcome> InstallMenuAsync(RunSchemaInstallRequest request, SchemaMenuDefinitionDto menu, CancellationToken cancellationToken)
    {
        if (request.DryRun)
        {
            return SchemaStepOutcome.AsSkipped($"DryRun activo. Se validaria/crearia el menu {menu.Code}.");
        }

        bool exists = await _sapSchemaService.MenuExistsAsync(request.CompanyCode, menu.Code, cancellationToken).ConfigureAwait(false);
        await _sapSchemaService.CreateOrUpdateMenuAsync(request.CompanyCode, menu, cancellationToken).ConfigureAwait(false);

        return exists
            ? SchemaStepOutcome.AsUpdated($"Menu {menu.Code} actualizado.")
            : SchemaStepOutcome.AsCreated($"Menu {menu.Code} creado.");
    }

    private async Task<SchemaStepOutcome> InstallSeedDataAsync(RunSchemaInstallRequest request, SchemaSeedDataDefinitionDto seedData, CancellationToken cancellationToken)
    {
        if (request.DryRun)
        {
            return SchemaStepOutcome.AsSkipped($"DryRun activo. Se validaria/crearia el dato inicial {seedData.TableCode}:{seedData.Code}.");
        }

        bool exists = await _sapSchemaService.SeedDataExistsAsync(request.CompanyCode, seedData.TableCode, seedData.Code, cancellationToken).ConfigureAwait(false);
        await _sapSchemaService.CreateOrUpdateSeedDataAsync(request.CompanyCode, seedData, cancellationToken).ConfigureAwait(false);

        return exists
            ? SchemaStepOutcome.AsUpdated($"Dato inicial {seedData.Code} actualizado en {seedData.TableCode}.")
            : SchemaStepOutcome.AsCreated($"Dato inicial {seedData.Code} creado en {seedData.TableCode}.");
    }

    private static async Task<bool> ExecuteStepAsync(
        List<SchemaInstallStepResultDto> steps,
        RunSchemaInstallRequest request,
        string stepCode,
        string stepName,
        string stepType,
        Func<Task<SchemaStepOutcome>> action)
    {
        try
        {
            SchemaStepOutcome outcome = await action().ConfigureAwait(false);
            steps.Add(new SchemaInstallStepResultDto(stepCode, stepName, stepType, true, outcome.Created, outcome.Updated, outcome.Skipped, outcome.Message));
            return true;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            steps.Add(new SchemaInstallStepResultDto(stepCode, stepName, stepType, false, false, false, false, ex.Message));
            return !request.StopOnFirstError;
        }
    }

    private SchemaInstallResultDto BuildResult(RunSchemaInstallRequest request, DateTimeOffset startedAt, IReadOnlyList<SchemaInstallStepResultDto> steps)
    {
        DateTimeOffset finishedAt = new(DateTime.SpecifyKind(_clock.UtcNow, DateTimeKind.Utc));
        return new SchemaInstallResultDto(steps.All(step => step.Success), request.DryRun, request.CompanyCode, startedAt, finishedAt, steps);
    }

    private sealed record SchemaStepOutcome(bool Created, bool Updated, bool Skipped, string Message)
    {
        public static SchemaStepOutcome AsCreated(string message) => new(true, false, false, message);
        public static SchemaStepOutcome AsUpdated(string message) => new(false, true, false, message);
        public static SchemaStepOutcome AsSkipped(string message) => new(false, false, true, message);
    }
}
