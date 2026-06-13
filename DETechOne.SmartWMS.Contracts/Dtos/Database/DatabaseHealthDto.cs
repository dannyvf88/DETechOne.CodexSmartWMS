namespace DETechOne.SmartWMS.Contracts.Dtos.Database;

public sealed record DatabaseHealthDto(
    bool Success,
    string Provider,
    string DatabaseType,
    string Message,
    DateTime CheckedAtUtc,
    long ElapsedMilliseconds);
