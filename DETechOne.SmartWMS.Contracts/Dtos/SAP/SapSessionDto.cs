namespace DETechOne.SmartWMS.Contracts.Dtos.SAP;

public sealed class SapSessionDto
{
    public bool IsAuthenticated { get; init; }
    public string SessionId { get; init; } = string.Empty;
    public string RouteId { get; init; } = string.Empty;
    public DateTime? ExpiresAtUtc { get; init; }
    public string Message { get; init; } = string.Empty;
}
