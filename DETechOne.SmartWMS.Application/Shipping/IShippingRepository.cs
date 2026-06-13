using DETechOne.SmartWMS.Domain.Entities.Shipping;

namespace DETechOne.SmartWMS.Application.Shipping;

public interface IShippingRepository
{
    Task<string> GetNextShippingNumberAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ShippingDocument>> GetOpenAsync(CancellationToken cancellationToken = default);
    Task<ShippingDocument?> GetByIdAsync(Guid shippingId, CancellationToken cancellationToken = default);
    Task AddAsync(ShippingDocument shippingDocument, CancellationToken cancellationToken = default);
    Task UpdateAsync(ShippingDocument shippingDocument, CancellationToken cancellationToken = default);
}
