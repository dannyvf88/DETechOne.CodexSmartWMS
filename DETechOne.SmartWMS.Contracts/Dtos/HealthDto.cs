namespace DETechOne.SmartWMS.Contracts.Dtos;

public sealed record HealthDto(string Service, string Status, DateTime UtcTime, string Version);
