namespace DETechOne.SmartWMS.Contracts.Dtos.Metadata;

public sealed record MetadataInstallResultDto(
    bool Success,
    bool DryRun,
    DateTimeOffset StartedAt,
    DateTimeOffset FinishedAt,
    IReadOnlyList<MetadataInstallStepResultDto> Steps)
{
    public int TotalSteps => Steps.Count;
    public int SuccessfulSteps => Steps.Count(step => step.Success);
    public int FailedSteps => Steps.Count(step => !step.Success);
}
