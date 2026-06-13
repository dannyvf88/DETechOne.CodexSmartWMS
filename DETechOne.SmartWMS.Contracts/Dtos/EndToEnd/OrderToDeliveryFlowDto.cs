namespace DETechOne.SmartWMS.Contracts.Dtos.EndToEnd;

public sealed class OrderToDeliveryFlowDto
{
    public Guid FlowId { get; init; }
    public string CorrelationId { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int SalesOrderDocEntry { get; init; }
    public int SalesOrderDocNum { get; init; }
    public string CustomerCode { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public Guid? PickingId { get; init; }
    public string? PickingNumber { get; init; }
    public Guid? PackingId { get; init; }
    public string? PackingNumber { get; init; }
    public Guid? ShippingId { get; init; }
    public string? ShippingNumber { get; init; }
    public int? DeliveryDocEntry { get; init; }
    public int? DeliveryDocNum { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public string CreatedBy { get; init; } = string.Empty;
    public IReadOnlyList<EndToEndFlowStepDto> Steps { get; init; } = Array.Empty<EndToEndFlowStepDto>();
}
