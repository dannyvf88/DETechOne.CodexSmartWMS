using DETechOne.SmartWMS.SAP.Configuration;
using Microsoft.Extensions.Options;

namespace DETechOne.SmartWMS.SAP.DIAPI;

public sealed class DiApiCompanyConnectionManager
{
    private readonly SapOptions _options;

    public DiApiCompanyConnectionManager(IOptions<SapOptions> options)
    {
        _options = options.Value;
    }

    public bool IsConfiguredForDiApi => !string.IsNullOrWhiteSpace(_options.Server)
        && !string.IsNullOrWhiteSpace(_options.CompanyDb)
        && !string.IsNullOrWhiteSpace(_options.UserName);

    public string GetConfigurationSummary() => IsConfiguredForDiApi
        ? $"DI API configuration present for company {_options.CompanyDb}."
        : "DI API configuration is incomplete.";
}
