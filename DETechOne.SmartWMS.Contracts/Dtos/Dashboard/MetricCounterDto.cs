namespace DETechOne.SmartWMS.Contracts.Dtos.Dashboard;

public sealed class MetricCounterDto
{
    public string Code { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public string? Unit { get; init; }
}
