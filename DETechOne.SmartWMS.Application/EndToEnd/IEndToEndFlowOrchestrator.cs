using DETechOne.SmartWMS.Contracts.Dtos.EndToEnd;
using DETechOne.SmartWMS.Contracts.Requests.EndToEnd;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.EndToEnd;

public interface IEndToEndFlowOrchestrator
{
    Task<Result<OrderToDeliveryFlowDto>> StartOrderToDeliveryFlowAsync(
        StartOrderToDeliveryFlowRequest request,
        string userName,
        CancellationToken cancellationToken = default);

    Task<Result<OrderToDeliveryFlowDto>> ExecuteAsync(
        ExecuteOrderToDeliveryFlowRequest request,
        string userName,
        CancellationToken cancellationToken = default);

    Task<Result<OrderToDeliveryFlowDto>> GetByIdAsync(
        Guid flowId,
        CancellationToken cancellationToken = default);
}
