using DETechOne.SmartWMS.Domain.Interfaces;

namespace DETechOne.SmartWMS.Infrastructure.Persistence;

public sealed class SmartWmsUnitOfWork : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0);
    }
}
