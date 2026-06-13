using System.Data.Common;

namespace DETechOne.SmartWMS.Application.Persistence;

public interface IDbConnectionFactory
{
    Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
}
