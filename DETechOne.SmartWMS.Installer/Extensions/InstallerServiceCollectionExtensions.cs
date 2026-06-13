using DETechOne.SmartWMS.Application.Metadata;
using DETechOne.SmartWMS.Application.Schema;
using DETechOne.SmartWMS.Installer.Definitions;
using DETechOne.SmartWMS.Installer.Definitions.Schema;
using DETechOne.SmartWMS.Installer.Services;
using DETechOne.SmartWMS.Installer.Services.Schema;
using DETechOne.SmartWMS.SAP.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DETechOne.SmartWMS.Installer.Extensions;

public static class InstallerServiceCollectionExtensions
{
    public static IServiceCollection AddSmartWmsInstaller(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSmartWmsSap(configuration);

        services.AddSingleton<IMetadataDefinitionProvider, SmartWmsMetadataDefinitionProvider>();
        services.AddScoped<IMetadataInstaller, SmartWmsMetadataInstaller>();

        services.AddSingleton<IDatabaseSchemaDefinitionProvider, SmartWmsSchemaDefinitionProvider>();
        services.AddScoped<IDatabaseSchemaInstaller, SmartWmsDatabaseSchemaInstaller>();

        return services;
    }
}
