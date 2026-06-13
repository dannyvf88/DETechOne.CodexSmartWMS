namespace DETechOne.SmartWMS.Contracts.Dtos.Stabilization;

public sealed record MvpReadinessDto(
    string Status,
    DateTime CheckedAtUtc,
    IReadOnlyCollection<MvpReadinessCheckDto> Checks)
{
    public int TotalChecks => Checks.Count;
    public int PassedChecks => Checks.Count(check => string.Equals(check.Status, "OK", StringComparison.OrdinalIgnoreCase));
    public int WarningChecks => Checks.Count(check => string.Equals(check.Status, "WARNING", StringComparison.OrdinalIgnoreCase));
    public int FailedChecks => Checks.Count(check => string.Equals(check.Status, "FAILED", StringComparison.OrdinalIgnoreCase));
}
