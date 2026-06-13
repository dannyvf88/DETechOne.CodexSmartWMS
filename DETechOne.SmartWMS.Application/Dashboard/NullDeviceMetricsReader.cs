using DETechOne.SmartWMS.Contracts.Dtos.Dashboard;

namespace DETechOne.SmartWMS.Application.Dashboard;

public sealed class NullDeviceMetricsReader : IDeviceMetricsReader
{
    public Task<DeviceMetricsDto> GetMetricsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DeviceMetricsDto());
    }
}
