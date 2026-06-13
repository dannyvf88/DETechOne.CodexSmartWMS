namespace DETechOne.SmartWMS.Application.Persistence;

public interface IQueryExecutor
{
    Task<int> ExecuteAsync(string sql, object? parameters = null, CancellationToken cancellationToken = default);
    Task<T?> QuerySingleOrDefaultAsync<T>(string sql, Func<IDataRecordMapper, T> map, object? parameters = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> QueryAsync<T>(string sql, Func<IDataRecordMapper, T> map, object? parameters = null, CancellationToken cancellationToken = default);
}
