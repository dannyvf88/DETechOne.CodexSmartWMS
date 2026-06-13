namespace DETechOne.SmartWMS.Contracts.Requests.Shipping;

public sealed class CreateShippingRequest
{
    public Guid PackingId { get; init; }
    public string PackingNumber { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public string CustomerCode { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public IReadOnlyList<CreateShippingLineRequest> Lines { get; init; } = Array.Empty<CreateShippingLineRequest>();
}
