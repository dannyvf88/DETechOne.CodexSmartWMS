using DETechOne.SmartWMS.Contracts.Dtos.EndToEnd;

namespace DETechOne.SmartWMS.Application.EndToEnd;

public static class OrderToDeliveryFlowMapper
{
    public static OrderToDeliveryFlowDto ToDto(OrderToDeliveryFlowState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        return new OrderToDeliveryFlowDto
        {
            FlowId = state.FlowId,
            CorrelationId = state.CorrelationId,
            Status = state.Status,
            SalesOrderDocEntry = state.SalesOrderDocEntry,
            SalesOrderDocNum = state.SalesOrderDocNum,
            CustomerCode = state.CustomerCode,
            CustomerName = state.CustomerName,
            WarehouseCode = state.WarehouseCode,
            PickingId = state.PickingId,
            PickingNumber = state.PickingNumber,
            PackingId = state.PackingId,
            PackingNumber = state.PackingNumber,
            ShippingId = state.ShippingId,
            ShippingNumber = state.ShippingNumber,
            DeliveryDocEntry = state.DeliveryDocEntry,
            DeliveryDocNum = state.DeliveryDocNum,
            CreatedAtUtc = state.CreatedAtUtc,
            CreatedBy = state.CreatedBy,
            Steps = state.Steps.ToArray()
        };
    }
}
