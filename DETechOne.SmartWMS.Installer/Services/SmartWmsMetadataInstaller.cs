using DETechOne.SmartWMS.Application.Common;
using DETechOne.SmartWMS.Application.Metadata;
using DETechOne.SmartWMS.Contracts.Dtos.Metadata;
using DETechOne.SmartWMS.Contracts.Requests.Metadata;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Installer.Services;

public sealed class SmartWmsMetadataInstaller : IMetadataInstaller
{
    private readonly IMetadataDefinitionProvider _definitionProvider;
    private readonly ISapMetadataService _sapMetadataService;
    private readonly IClock _clock;

    public SmartWmsMetadataInstaller(
        IMetadataDefinitionProvider definitionProvider,
        ISapMetadataService sapMetadataService,
        IClock clock)
    {
        _definitionProvider = definitionProvider;
        _sapMetadataService = sapMetadataService;
        _clock = clock;
    }

    public async Task<MetadataInstallResultDto> InstallAsync(RunMetadataInstallRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.CompanyCode);

        DateTimeOffset startedAt = new(DateTime.SpecifyKind(_clock.UtcNow, DateTimeKind.Utc));
        var steps = new List<MetadataInstallStepResultDto>();

        foreach (MetadataObjectDefinitionDto definition in _definitionProvider.GetDefinitions())
        {
            try
            {
                if (request.DryRun)
                {
                    steps.Add(new MetadataInstallStepResultDto(
                        definition.Code,
                        definition.Name,
                        definition.ObjectType,
                        MetadataInstallStatus.Skipped,
                        true,
                        "DryRun activo. No se realizaron cambios en SAP Business One."));
                    continue;
                }

                bool exists = await _sapMetadataService.ExistsAsync(definition, request.CompanyCode, cancellationToken).ConfigureAwait(false);
                await _sapMetadataService.CreateOrUpdateAsync(definition, request.CompanyCode, cancellationToken).ConfigureAwait(false);

                steps.Add(new MetadataInstallStepResultDto(
                    definition.Code,
                    definition.Name,
                    definition.ObjectType,
                    exists ? MetadataInstallStatus.Updated : MetadataInstallStatus.Created,
                    true,
                    exists ? "Metadata actualizada correctamente." : "Metadata creada correctamente."));
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                steps.Add(new MetadataInstallStepResultDto(
                    definition.Code,
                    definition.Name,
                    definition.ObjectType,
                    MetadataInstallStatus.Failed,
                    false,
                    ex.Message));

                if (request.StopOnFirstError)
                {
                    break;
                }
            }
        }

        DateTimeOffset finishedAt = new(DateTime.SpecifyKind(_clock.UtcNow, DateTimeKind.Utc));
        bool success = steps.All(step => step.Success);

        return new MetadataInstallResultDto(success, request.DryRun, startedAt, finishedAt, steps);
    }
}
