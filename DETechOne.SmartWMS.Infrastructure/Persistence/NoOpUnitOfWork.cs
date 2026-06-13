using DETechOne.SmartWMS.Domain.Interfaces;

namespace DETechOne.SmartWMS.Infrastructure.Persistence;

public sealed class NoOpUnitOfWork : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0);
    }
}
