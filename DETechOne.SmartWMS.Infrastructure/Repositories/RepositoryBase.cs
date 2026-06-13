using DETechOne.SmartWMS.Application.Persistence;
using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Interfaces;

namespace DETechOne.SmartWMS.Infrastructure.Repositories;

public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    protected RepositoryBase(IQueryExecutor queryExecutor)
    {
        QueryExecutor = queryExecutor;
    }

    protected IQueryExecutor QueryExecutor { get; }

    public abstract Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public abstract Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken cancellationToken = default);
    public abstract Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    public abstract Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    public abstract Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}
