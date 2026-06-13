namespace DETechOne.SmartWMS.Contracts.Requests.Metadata;

public sealed record RunMetadataInstallRequest(
    string CompanyCode,
    bool DryRun = true,
    bool StopOnFirstError = true);
