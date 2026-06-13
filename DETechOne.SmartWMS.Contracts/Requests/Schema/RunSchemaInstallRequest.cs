namespace DETechOne.SmartWMS.Contracts.Requests.Schema;

public sealed record RunSchemaInstallRequest(
    string CompanyCode,
    bool DryRun = true,
    bool StopOnFirstError = true,
    bool InstallTables = true,
    bool InstallUdos = true,
    bool InstallPermissions = true,
    bool InstallMenus = true,
    bool InstallSeedData = true);
