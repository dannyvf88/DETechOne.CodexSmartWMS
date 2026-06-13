using System.Diagnostics;
using DETechOne.SmartWMS.Application.Persistence;
using DETechOne.SmartWMS.Infrastructure.Configuration.Database;
using Microsoft.Extensions.Options;

namespace DETechOne.SmartWMS.Infrastructure.Persistence;

public sealed class DatabaseHealthCheck : IDatabaseHealthCheck
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly DatabaseOptions _options;

    public DatabaseHealthCheck(IDbConnectionFactory connectionFactory, IOptions<DatabaseOptions> options)
    {
        _connectionFactory = connectionFactory;
        _options = options.Value;
    }

    public async Task<DatabaseHealthResult> CheckAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        if (!_options.IsConfigured)
        {
            stopwatch.Stop();
            return new DatabaseHealthResult(false, _options.Provider, GetDatabaseType(), "Database connection string is not configured.", DateTime.UtcNow, stopwatch.ElapsedMilliseconds);
        }

        try
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
            await using var command = connection.CreateCommand();
            command.CommandText = GetValidationQuery();
            command.CommandTimeout = _options.CommandTimeoutSeconds;
            await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);

            stopwatch.Stop();
            return new DatabaseHealthResult(true, _options.Provider, GetDatabaseType(), "Database connection OK.", DateTime.UtcNow, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new DatabaseHealthResult(false, _options.Provider, GetDatabaseType(), ex.Message, DateTime.UtcNow, stopwatch.ElapsedMilliseconds);
        }
    }

    private string GetValidationQuery()
    {
        return string.Equals(_options.Provider, DatabaseProviderNames.Hana, StringComparison.OrdinalIgnoreCase)
            ? "SELECT 1 FROM DUMMY"
            : "SELECT 1";
    }

    private string GetDatabaseType()
    {
        return string.Equals(_options.Provider, DatabaseProviderNames.Hana, StringComparison.OrdinalIgnoreCase)
            ? "SAP HANA"
            : "SQL Server";
    }
}
