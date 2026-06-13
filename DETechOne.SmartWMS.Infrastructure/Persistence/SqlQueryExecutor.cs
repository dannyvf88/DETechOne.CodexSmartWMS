using System.Data;
using DETechOne.SmartWMS.Application.Persistence;
using DETechOne.SmartWMS.Infrastructure.Configuration.Database;
using Microsoft.Extensions.Options;

namespace DETechOne.SmartWMS.Infrastructure.Persistence;

public sealed class SqlQueryExecutor : IQueryExecutor
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly DatabaseOptions _options;

    public SqlQueryExecutor(IDbConnectionFactory connectionFactory, IOptions<DatabaseOptions> options)
    {
        _connectionFactory = connectionFactory;
        _options = options.Value;
    }

    public async Task<int> ExecuteAsync(string sql, object? parameters = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sql);

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.CommandType = CommandType.Text;
        command.CommandTimeout = _options.CommandTimeoutSeconds;
        DbParameterBinder.Bind(command, parameters);

        return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, Func<IDataRecordMapper, T> map, object? parameters = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sql);
        ArgumentNullException.ThrowIfNull(map);

        IReadOnlyList<T> rows = await QueryAsync(sql, map, parameters, cancellationToken).ConfigureAwait(false);

        if (rows.Count > 1)
        {
            throw new InvalidOperationException("The query returned more than one row.");
        }

        return rows.Count == 0 ? default : rows[0];
    }

    public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, Func<IDataRecordMapper, T> map, object? parameters = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sql);
        ArgumentNullException.ThrowIfNull(map);

        var result = new List<T>();

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.CommandType = CommandType.Text;
        command.CommandTimeout = _options.CommandTimeoutSeconds;
        DbParameterBinder.Bind(command, parameters);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            result.Add(map(new DataRecordMapper(reader)));
        }

        return result;
    }
}
