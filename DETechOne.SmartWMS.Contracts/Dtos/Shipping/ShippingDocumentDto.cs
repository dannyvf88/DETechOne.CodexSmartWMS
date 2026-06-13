namespace DETechOne.SmartWMS.Contracts.Dtos.Shipping;

public sealed class ShippingDocumentDto
{
    public Guid Id { get; init; }
    public string ShippingNumber { get; init; } = string.Empty;
    public Guid PackingId { get; init; }
    public string PackingNumber { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public string CustomerCode { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public string RequestedBy { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal PackedQuantity { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? ConfirmedAtUtc { get; init; }
    public string? ConfirmedBy { get; init; }
    public int? DeliveryDocEntry { get; init; }
    public int? DeliveryDocNum { get; init; }
    public DateTime? DeliveryCreatedAtUtc { get; init; }
    public string? DeliveryCreatedBy { get; init; }
    public DateTime? CancelledAtUtc { get; init; }
    public string? CancelledBy { get; init; }
    public string? CancelReason { get; init; }
    public IReadOnlyList<ShippingLineDto> Lines { get; init; } = Array.Empty<ShippingLineDto>();
}
