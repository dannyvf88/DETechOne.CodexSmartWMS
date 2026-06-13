using DETechOne.SmartWMS.Application.SAP;
using DETechOne.SmartWMS.Contracts.Dtos.SAP;
using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.SAP.Configuration;
using Microsoft.Extensions.Options;

namespace DETechOne.SmartWMS.SAP.Connection;

public sealed class SapConnectionManager : ISapConnectionManager
{
    private readonly SapOptions _options;

    public SapConnectionManager(IOptions<SapOptions> options)
    {
        _options = options.Value;
    }

    public Task<Result<SapConnectionStatusDto>> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        SapConnectionStatusDto status = new()
        {
            IsConfigured = _options.IsConfigured,
            IsConnected = false,
            Mode = _options.Mode,
            CompanyDb = _options.CompanyDb,
            Server = _options.Server,
            ServiceLayerBaseUrl = _options.ServiceLayerBaseUrl,
            Message = _options.IsConfigured
                ? "SAP configuration is present. Runtime connection validation is pending real SAP adapter activation."
                : "SAP integration is not configured. Complete the SAP section in appsettings.json."
        };

        return Task.FromResult(Result<SapConnectionStatusDto>.Ok(status));
    }
}
