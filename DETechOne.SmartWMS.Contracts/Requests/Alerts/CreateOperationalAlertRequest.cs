namespace DETechOne.SmartWMS.Contracts.Requests.Alerts;

public sealed class CreateOperationalAlertRequest
{
    public string Severity { get; init; } = "Warning";
    public string Source { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string? EntityType { get; init; }
    public string? EntityId { get; init; }
    public string? DeviceCode { get; init; }
    public string? CorrelationId { get; init; }
}
