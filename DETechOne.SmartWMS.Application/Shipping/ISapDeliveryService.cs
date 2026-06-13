using DETechOne.SmartWMS.Contracts.Dtos.Shipping;
using DETechOne.SmartWMS.Domain.Entities.Shipping;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.Shipping;

public interface ISapDeliveryService
{
    Task<Result<SapDeliveryResultDto>> CreateDeliveryAsync(ShippingDocument shippingDocument, string userName, CancellationToken cancellationToken = default);
}
