using DETechOne.SmartWMS.Device.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DETechOne.SmartWMS.Device.Extensions;

public static class DeviceServiceCollectionExtensions
{
    public static IServiceCollection AddSmartWmsDevice(this IServiceCollection services)
    {
        services.AddSingleton<IDeviceRegistryService, InMemoryDeviceRegistryService>();
        services.AddSingleton<IScannerSessionService, InMemoryScannerSessionService>();
        return services;
    }
}
