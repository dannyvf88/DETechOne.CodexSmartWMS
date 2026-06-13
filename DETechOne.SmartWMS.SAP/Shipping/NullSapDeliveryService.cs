using DETechOne.SmartWMS.Application.Shipping;
using DETechOne.SmartWMS.Contracts.Dtos.Shipping;
using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Entities.Shipping;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.SAP.Shipping;

public sealed class NullSapDeliveryService : ISapDeliveryService
{
    public Task<Result<SapDeliveryResultDto>> CreateDeliveryAsync(ShippingDocument shippingDocument, string userName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(shippingDocument);

        if (shippingDocument.Status != ShippingStatus.Confirmed)
        {
            return Task.FromResult(Result<SapDeliveryResultDto>.Fail("SAP_DELIVERY_VALIDATION", "Shipping document must be confirmed before creating SAP delivery."));
        }

        var simulatedDocNum = Math.Abs(shippingDocument.Id.GetHashCode());
        var result = new SapDeliveryResultDto
        {
            DocEntry = simulatedDocNum == 0 ? 1 : simulatedDocNum,
            DocNum = simulatedDocNum == 0 ? 1 : simulatedDocNum,
            Message = "SAP delivery simulated successfully. Replace NullSapDeliveryService with DI API implementation for production."
        };

        return Task.FromResult(Result<SapDeliveryResultDto>.Ok(result, result.Message));
    }
}
