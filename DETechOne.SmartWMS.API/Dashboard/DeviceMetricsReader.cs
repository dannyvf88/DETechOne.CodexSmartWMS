using DETechOne.SmartWMS.Application.Dashboard;
using DETechOne.SmartWMS.Contracts.Dtos.Dashboard;
using DETechOne.SmartWMS.Contracts.Dtos.Device;
using DETechOne.SmartWMS.Device.Services;

namespace DETechOne.SmartWMS.API.Dashboard;

public sealed class DeviceMetricsReader : IDeviceMetricsReader
{
    private readonly IDeviceRegistryService _deviceRegistryService;

    public DeviceMetricsReader(IDeviceRegistryService deviceRegistryService)
    {
        _deviceRegistryService = deviceRegistryService;
    }

    public async Task<DeviceMetricsDto> GetMetricsAsync(CancellationToken cancellationToken = default)
    {
        var result = await _deviceRegistryService.GetOnlineAsync(cancellationToken).ConfigureAwait(false);
        IReadOnlyCollection<DeviceRegistrationDto> onlineDevices = result.Value ?? Array.Empty<DeviceRegistrationDto>();

        return new DeviceMetricsDto
        {
            Registered = onlineDevices.Count,
            Online = onlineDevices.Count,
            Offline = 0,
            Blocked = 0,
            LastHeartbeatAtUtc = onlineDevices
                .Where(device => device.LastHeartbeatAtUtc.HasValue)
                .Select(device => device.LastHeartbeatAtUtc)
                .OrderByDescending(value => value)
                .FirstOrDefault()
        };
    }
}
