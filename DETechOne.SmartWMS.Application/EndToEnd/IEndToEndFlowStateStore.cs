namespace DETechOne.SmartWMS.Application.EndToEnd;

public interface IEndToEndFlowStateStore
{
    Task SaveAsync(OrderToDeliveryFlowState state, CancellationToken cancellationToken = default);
    Task<OrderToDeliveryFlowState?> GetByIdAsync(Guid flowId, CancellationToken cancellationToken = default);
}
