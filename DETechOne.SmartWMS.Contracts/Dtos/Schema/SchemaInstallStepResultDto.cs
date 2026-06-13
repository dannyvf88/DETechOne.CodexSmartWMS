namespace DETechOne.SmartWMS.Contracts.Dtos.Schema;

public sealed record SchemaInstallStepResultDto(
    string StepCode,
    string StepName,
    string StepType,
    bool Success,
    bool Created,
    bool Updated,
    bool Skipped,
    string Message);
