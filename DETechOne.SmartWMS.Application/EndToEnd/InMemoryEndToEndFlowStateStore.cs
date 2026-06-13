using System.Collections.Concurrent;

namespace DETechOne.SmartWMS.Application.EndToEnd;

public sealed class InMemoryEndToEndFlowStateStore : IEndToEndFlowStateStore
{
    private readonly ConcurrentDictionary<Guid, OrderToDeliveryFlowState> _flows = new();

    public Task SaveAsync(OrderToDeliveryFlowState state, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(state);

        _flows[state.FlowId] = state;
        return Task.CompletedTask;
    }

    public Task<OrderToDeliveryFlowState?> GetByIdAsync(Guid flowId, CancellationToken cancellationToken = default)
    {
        _flows.TryGetValue(flowId, out OrderToDeliveryFlowState? state);
        return Task.FromResult(state);
    }
}
