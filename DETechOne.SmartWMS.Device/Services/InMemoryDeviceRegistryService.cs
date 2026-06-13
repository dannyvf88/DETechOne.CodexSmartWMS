using System.Collections.Concurrent;
using DETechOne.SmartWMS.Contracts.Dtos.Device;
using DETechOne.SmartWMS.Contracts.Requests.Device;
using DETechOne.SmartWMS.Device.Mapping;
using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Entities.Device;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Device.Services;

public sealed class InMemoryDeviceRegistryService : IDeviceRegistryService
{
    private static readonly ConcurrentDictionary<string, WmsDevice> Devices = new(StringComparer.OrdinalIgnoreCase);

    public Task<Result<DeviceRegistrationDto>> RegisterAsync(RegisterDeviceRequest request, string userName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(request.DeviceCode))
        {
            return Task.FromResult(Result<DeviceRegistrationDto>.Fail("DEVICE_CODE_REQUIRED", "Device code is required."));
        }

        var device = Devices.AddOrUpdate(
            request.DeviceCode.Trim(),
            _ => CreateDevice(request, userName),
            (_, existing) =>
            {
                existing.RegisterHeartbeat(request.IpAddress, userName);
                return existing;
            });

        return Task.FromResult(Result<DeviceRegistrationDto>.Ok(DeviceMapper.ToDto(device), "Device registered."));
    }

    public Task<Result<DeviceHeartbeatDto>> HeartbeatAsync(DeviceHeartbeatRequest request, string userName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(request.DeviceCode))
        {
            return Task.FromResult(Result<DeviceHeartbeatDto>.Fail("DEVICE_CODE_REQUIRED", "Device code is required."));
        }

        if (!Devices.TryGetValue(request.DeviceCode.Trim(), out var device))
        {
            return Task.FromResult(Result<DeviceHeartbeatDto>.Fail("DEVICE_NOT_REGISTERED", "Device is not registered."));
        }

        device.RegisterHeartbeat(request.IpAddress, userName);

        var heartbeat = new DeviceHeartbeatDto
        {
            DeviceCode = device.DeviceCode,
            Status = DeviceStatus.Online,
            HeartbeatAtUtc = device.LastHeartbeatAtUtc ?? DateTime.UtcNow,
            AppVersion = string.IsNullOrWhiteSpace(request.AppVersion) ? device.AppVersion : request.AppVersion
        };

        return Task.FromResult(Result<DeviceHeartbeatDto>.Ok(heartbeat, "Heartbeat received."));
    }

    public Task<Result<DeviceRegistrationDto>> GetAsync(string deviceCode, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(deviceCode))
        {
            return Task.FromResult(Result<DeviceRegistrationDto>.Fail("DEVICE_CODE_REQUIRED", "Device code is required."));
        }

        if (!Devices.TryGetValue(deviceCode.Trim(), out var device))
        {
            return Task.FromResult(Result<DeviceRegistrationDto>.Fail("DEVICE_NOT_FOUND", "Device was not found."));
        }

        return Task.FromResult(Result<DeviceRegistrationDto>.Ok(DeviceMapper.ToDto(device)));
    }

    public Task<Result<IReadOnlyCollection<DeviceRegistrationDto>>> GetOnlineAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var onlineDevices = Devices.Values
            .Where(device => device.Status == DeviceStatus.Online)
            .Select(DeviceMapper.ToDto)
            .OrderBy(device => device.DeviceCode, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return Task.FromResult(Result<IReadOnlyCollection<DeviceRegistrationDto>>.Ok(onlineDevices));
    }

    private static WmsDevice CreateDevice(RegisterDeviceRequest request, string userName)
    {
        var device = new WmsDevice(
            request.DeviceCode,
            request.Name,
            request.DeviceType,
            request.Model,
            request.Platform,
            request.AppVersion,
            request.SerialNumber,
            request.WarehouseCode,
            userName);

        device.RegisterHeartbeat(request.IpAddress, userName);
        return device;
    }
}
