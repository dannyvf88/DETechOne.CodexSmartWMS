using DETechOne.SmartWMS.Contracts.Dtos.EndToEnd;

namespace DETechOne.SmartWMS.Application.EndToEnd;

public sealed class OrderToDeliveryFlowState
{
    public Guid FlowId { get; init; }
    public string CorrelationId { get; init; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int SalesOrderDocEntry { get; init; }
    public int SalesOrderDocNum { get; init; }
    public string CustomerCode { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public Guid? PickingId { get; set; }
    public string? PickingNumber { get; set; }
    public Guid? PackingId { get; set; }
    public string? PackingNumber { get; set; }
    public Guid? ShippingId { get; set; }
    public string? ShippingNumber { get; set; }
    public int? DeliveryDocEntry { get; set; }
    public int? DeliveryDocNum { get; set; }
    public DateTime CreatedAtUtc { get; init; }
    public string CreatedBy { get; init; } = string.Empty;
    public List<EndToEndFlowStepDto> Steps { get; } = new();
}
