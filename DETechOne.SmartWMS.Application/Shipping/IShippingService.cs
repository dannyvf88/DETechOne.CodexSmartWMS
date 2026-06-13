using DETechOne.SmartWMS.Contracts.Dtos.Shipping;
using DETechOne.SmartWMS.Contracts.Requests.Shipping;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.Shipping;

public interface IShippingService
{
    Task<Result<IReadOnlyList<ShippingDocumentDto>>> GetOpenAsync(CancellationToken cancellationToken = default);
    Task<Result<ShippingDocumentDto>> GetByIdAsync(Guid shippingId, CancellationToken cancellationToken = default);
    Task<Result<ShippingDocumentDto>> CreateAsync(CreateShippingRequest request, string? userName, CancellationToken cancellationToken = default);
    Task<Result<ShippingDocumentDto>> ConfirmAsync(ConfirmShippingRequest request, string? userName, CancellationToken cancellationToken = default);
    Task<Result<ShippingDocumentDto>> CreateSapDeliveryAsync(CreateSapDeliveryRequest request, string? userName, CancellationToken cancellationToken = default);
    Task<Result<ShippingDocumentDto>> CancelAsync(CancelShippingRequest request, string? userName, CancellationToken cancellationToken = default);
}
