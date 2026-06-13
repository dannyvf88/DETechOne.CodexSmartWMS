using DETechOne.SmartWMS.Contracts.Dtos.Dashboard;

namespace DETechOne.SmartWMS.Application.Dashboard;

public interface IDeviceMetricsReader
{
    Task<DeviceMetricsDto> GetMetricsAsync(CancellationToken cancellationToken = default);
}
