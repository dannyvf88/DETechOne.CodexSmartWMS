namespace DETechOne.SmartWMS.Contracts.Requests.Shipping;

public sealed class CancelShippingRequest
{
    public Guid ShippingId { get; init; }
    public string Reason { get; init; } = string.Empty;
}
