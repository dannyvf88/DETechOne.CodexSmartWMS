using System.Data.Common;
using DETechOne.SmartWMS.Application.Persistence;
using DETechOne.SmartWMS.Infrastructure.Configuration.Database;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace DETechOne.SmartWMS.Infrastructure.Persistence;

public sealed class SmartWmsDbConnectionFactory : IDbConnectionFactory
{
    private readonly DatabaseOptions _options;

    public SmartWmsDbConnectionFactory(IOptions<DatabaseOptions> options)
    {
        _options = options.Value;
    }

    public async Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.IsConfigured)
        {
            throw new InvalidOperationException("Database connection string is not configured. Configure Database:ConnectionString in appsettings.json.");
        }

        DbConnection connection = CreateConnection();
        connection.ConnectionString = _options.ConnectionString;
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        return connection;
    }

    private DbConnection CreateConnection()
    {
        if (string.Equals(_options.Provider, DatabaseProviderNames.SqlServer, StringComparison.OrdinalIgnoreCase))
        {
            return new SqlConnection();
        }

        if (string.Equals(_options.Provider, DatabaseProviderNames.Hana, StringComparison.OrdinalIgnoreCase))
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory(_options.HanaProviderInvariantName);
            return factory.CreateConnection()
                ?? throw new InvalidOperationException($"The provider '{_options.HanaProviderInvariantName}' did not create a valid connection.");
        }

        throw new NotSupportedException($"Database provider '{_options.Provider}' is not supported.");
    }
}
