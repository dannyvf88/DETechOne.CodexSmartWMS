namespace DETechOne.SmartWMS.Contracts.Dtos.Stabilization;

public sealed record MvpReadinessCheckDto(
    string Code,
    string Name,
    string Status,
    string Message,
    DateTime CheckedAtUtc,
    long ElapsedMilliseconds);
