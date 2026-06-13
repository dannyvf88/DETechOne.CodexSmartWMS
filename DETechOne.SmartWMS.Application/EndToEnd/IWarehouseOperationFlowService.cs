using DETechOne.SmartWMS.Contracts.Dtos.Packing;
using DETechOne.SmartWMS.Contracts.Dtos.Picking;
using DETechOne.SmartWMS.Contracts.Dtos.Shipping;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.EndToEnd;

/// <summary>
/// Integration boundary between the End-to-End orchestrator and warehouse operational engines.
/// It centralizes Picking, Packing and Shipping transitions so the orchestrator does not need to know
/// the internal request mapping for each engine.
/// </summary>
public interface IWarehouseOperationFlowService
{
    Task<Result<PickingDocumentDto>> CompletePickingAsync(
        Guid pickingId,
        bool autoCompletePicking,
        string userName,
        CancellationToken cancellationToken = default);

    Task<Result<PackingDocumentDto>> CreatePackingFromPickingAsync(
        PickingDocumentDto picking,
        string userName,
        CancellationToken cancellationToken = default);

    Task<Result<PackingDocumentDto>> CompletePackingAsync(
        Guid packingId,
        bool autoCompletePacking,
        string? packageCode,
        string userName,
        CancellationToken cancellationToken = default);

    Task<Result<ShippingDocumentDto>> CreateShippingFromPackingAsync(
        PackingDocumentDto packing,
        string customerCode,
        string customerName,
        string userName,
        CancellationToken cancellationToken = default);

    Task<Result<ShippingDocumentDto>> ConfirmShippingAsync(
        Guid shippingId,
        bool autoConfirmShipping,
        string userName,
        CancellationToken cancellationToken = default);
}
