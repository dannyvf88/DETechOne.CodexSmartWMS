using DETechOne.SmartWMS.Application.Metadata;
using DETechOne.SmartWMS.Application.SAP;
using DETechOne.SmartWMS.Application.Schema;
using DETechOne.SmartWMS.Application.Shipping;
using DETechOne.SmartWMS.SAP.Configuration;
using DETechOne.SmartWMS.SAP.Connection;
using DETechOne.SmartWMS.SAP.Delivery;
using DETechOne.SmartWMS.SAP.DIAPI;
using DETechOne.SmartWMS.SAP.Inventory;
using DETechOne.SmartWMS.SAP.MasterData;
using DETechOne.SmartWMS.SAP.Metadata;
using DETechOne.SmartWMS.SAP.Sales;
using DETechOne.SmartWMS.SAP.ServiceLayer;
using DETechOne.SmartWMS.SAP.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DETechOne.SmartWMS.SAP.Extensions;

public static class SapServiceCollectionExtensions
{
    public static IServiceCollection AddSmartWmsSap(this IServiceCollection services, IConfiguration configuration)
    {
        SapOptions options = BindSapOptions(configuration);

        services.AddSingleton<IOptions<SapOptions>>(_ => Options.Create(options));
        services.AddScoped<ISapMetadataService, NullSapMetadataService>();
        services.AddScoped<ISapSchemaService, NullSapSchemaService>();
        services.AddScoped<ISapConnectionManager, SapConnectionManager>();
        services.AddScoped<ISapSessionManager, ServiceLayerSessionManager>();
        services.AddScoped<ISapBusinessPartnerReader, SapBusinessPartnerReader>();
        services.AddScoped<ISapSalesOrderReader, SapSalesOrderReader>();
        services.AddScoped<ISapItemMasterReader, SapItemMasterReader>();
        services.AddScoped<ISapWarehouseReader, SapWarehouseReader>();
        services.AddScoped<ISapInventoryTransferService, SapInventoryTransferService>();
        services.AddScoped<ISapDeliveryService, SapDeliveryService>();
        services.AddSingleton<DiApiCompanyConnectionManager>();

        services.AddHttpClient<IServiceLayerClient, ServiceLayerClient>((_, client) =>
        {
            if (!string.IsNullOrWhiteSpace(options.ServiceLayerBaseUrl))
            {
                client.BaseAddress = new Uri(options.ServiceLayerBaseUrl.TrimEnd('/') + "/");
            }

            client.Timeout = TimeSpan.FromSeconds(options.ServiceLayerTimeoutSeconds <= 0 ? 100 : options.ServiceLayerTimeoutSeconds);
        });

        return services;
    }

    private static SapOptions BindSapOptions(IConfiguration configuration)
    {
        IConfigurationSection section = configuration.GetSection(SapOptions.SectionName);

        return new SapOptions
        {
            Mode = section[nameof(SapOptions.Mode)] ?? "Disabled",
            Server = section[nameof(SapOptions.Server)] ?? string.Empty,
            CompanyDb = section[nameof(SapOptions.CompanyDb)] ?? string.Empty,
            DbServerType = section[nameof(SapOptions.DbServerType)] ?? string.Empty,
            DbUserName = section[nameof(SapOptions.DbUserName)] ?? string.Empty,
            DbPassword = section[nameof(SapOptions.DbPassword)] ?? string.Empty,
            UserName = section[nameof(SapOptions.UserName)] ?? string.Empty,
            Password = section[nameof(SapOptions.Password)] ?? string.Empty,
            LicenseServer = section[nameof(SapOptions.LicenseServer)] ?? string.Empty,
            ServiceLayerBaseUrl = section[nameof(SapOptions.ServiceLayerBaseUrl)] ?? string.Empty,
            ServiceLayerTimeoutSeconds = int.TryParse(section[nameof(SapOptions.ServiceLayerTimeoutSeconds)], out int timeout) ? timeout : 100,
            TrustServiceLayerCertificate = bool.TryParse(section[nameof(SapOptions.TrustServiceLayerCertificate)], out bool trust) && trust
        };
    }
}
