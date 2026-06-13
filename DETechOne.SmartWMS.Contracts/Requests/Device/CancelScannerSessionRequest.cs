namespace DETechOne.SmartWMS.Contracts.Requests.Device;

public sealed class CancelScannerSessionRequest
{
    public string Reason { get; init; } = "Cancelled by operator.";
}
