using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Domain.Entities.Device;

public sealed class WmsDevice : BaseEntity
{
    private WmsDevice()
    {
        DeviceCode = string.Empty;
        Name = string.Empty;
        Model = string.Empty;
        Platform = string.Empty;
        AppVersion = string.Empty;
    }

    public WmsDevice(
        string deviceCode,
        string name,
        DeviceType deviceType,
        string model,
        string platform,
        string appVersion,
        string? serialNumber,
        string? warehouseCode,
        string createdBy)
    {
        DeviceCode = string.IsNullOrWhiteSpace(deviceCode) ? throw new ArgumentException("Device code is required.", nameof(deviceCode)) : deviceCode.Trim();
        Name = string.IsNullOrWhiteSpace(name) ? DeviceCode : name.Trim();
        DeviceType = deviceType;
        Model = model?.Trim() ?? string.Empty;
        Platform = platform?.Trim() ?? string.Empty;
        AppVersion = appVersion?.Trim() ?? string.Empty;
        SerialNumber = string.IsNullOrWhiteSpace(serialNumber) ? null : serialNumber.Trim();
        WarehouseCode = string.IsNullOrWhiteSpace(warehouseCode) ? null : warehouseCode.Trim();
        Status = DeviceStatus.Registered;
        CreatedBy = createdBy;
    }

    public string DeviceCode { get; private set; }
    public string Name { get; private set; }
    public DeviceType DeviceType { get; private set; }
    public string Model { get; private set; }
    public string Platform { get; private set; }
    public string AppVersion { get; private set; }
    public string? SerialNumber { get; private set; }
    public string? WarehouseCode { get; private set; }
    public DeviceStatus Status { get; private set; }
    public DateTime? LastHeartbeatAtUtc { get; private set; }
    public string? LastIpAddress { get; private set; }
    public string? LastUserName { get; private set; }

    public void RegisterHeartbeat(string? ipAddress, string? userName)
    {
        LastHeartbeatAtUtc = DateTime.UtcNow;
        LastIpAddress = string.IsNullOrWhiteSpace(ipAddress) ? null : ipAddress.Trim();
        LastUserName = string.IsNullOrWhiteSpace(userName) ? LastUserName : userName.Trim();
        Status = DeviceStatus.Online;
        MarkUpdated(userName);
    }

    public void MarkOffline(string? userName)
    {
        Status = DeviceStatus.Offline;
        MarkUpdated(userName);
    }

    public void Block(string? userName)
    {
        Status = DeviceStatus.Blocked;
        MarkUpdated(userName);
    }
}
