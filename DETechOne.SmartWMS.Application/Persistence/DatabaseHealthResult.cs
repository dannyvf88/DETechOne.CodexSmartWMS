namespace DETechOne.SmartWMS.Application.Persistence;

public sealed record DatabaseHealthResult(
    bool Success,
    string Provider,
    string DatabaseType,
    string Message,
    DateTime CheckedAtUtc,
    long ElapsedMilliseconds);
