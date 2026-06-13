namespace DETechOne.SmartWMS.Contracts.Dtos.Schema;

public sealed record SchemaInstallResultDto(
    bool Success,
    bool DryRun,
    string CompanyCode,
    DateTimeOffset StartedAt,
    DateTimeOffset FinishedAt,
    IReadOnlyList<SchemaInstallStepResultDto> Steps);
