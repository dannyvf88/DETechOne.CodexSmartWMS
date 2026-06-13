using DETechOne.SmartWMS.Contracts.Dtos.Device;
using DETechOne.SmartWMS.Contracts.Requests.Device;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Device.Services;

public interface IDeviceRegistryService
{
    Task<Result<DeviceRegistrationDto>> RegisterAsync(RegisterDeviceRequest request, string userName, CancellationToken cancellationToken = default);
    Task<Result<DeviceHeartbeatDto>> HeartbeatAsync(DeviceHeartbeatRequest request, string userName, CancellationToken cancellationToken = default);
    Task<Result<DeviceRegistrationDto>> GetAsync(string deviceCode, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<DeviceRegistrationDto>>> GetOnlineAsync(CancellationToken cancellationToken = default);
}
