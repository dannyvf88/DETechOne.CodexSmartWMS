using DETechOne.SmartWMS.Application.SAP;
using DETechOne.SmartWMS.Contracts.Dtos.SAP;
using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.SAP.Configuration;
using Microsoft.Extensions.Options;

namespace DETechOne.SmartWMS.SAP.ServiceLayer;

public sealed class ServiceLayerSessionManager : ISapSessionManager
{
    private readonly SapOptions _options;

    public ServiceLayerSessionManager(IOptions<SapOptions> options)
    {
        _options = options.Value;
    }

    public Task<Result<SapSessionDto>> LoginAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.IsConfigured || string.IsNullOrWhiteSpace(_options.ServiceLayerBaseUrl))
        {
            return Task.FromResult(Result<SapSessionDto>.Fail(
                "SAP_NOT_CONFIGURED",
                "Service Layer is not configured. Set SAP:ServiceLayerBaseUrl, SAP:CompanyDb, SAP:UserName and SAP:Password."));
        }

        SapSessionDto dto = new()
        {
            IsAuthenticated = false,
            Message = "Service Layer login contract is ready. Enable the real HTTP login implementation when SAP connectivity is available."
        };

        return Task.FromResult(Result<SapSessionDto>.Ok(dto));
    }

    public Task<Result> LogoutAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Ok("Service Layer logout contract is ready."));
    }
}
