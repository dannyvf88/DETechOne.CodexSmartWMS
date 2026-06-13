namespace DETechOne.SmartWMS.Contracts.Dtos.EndToEnd;

public sealed class EndToEndFlowStepDto
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? Message { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
}
