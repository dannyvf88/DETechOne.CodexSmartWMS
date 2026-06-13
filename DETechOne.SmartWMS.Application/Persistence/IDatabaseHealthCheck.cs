namespace DETechOne.SmartWMS.Application.Persistence;

public interface IDatabaseHealthCheck
{
    Task<DatabaseHealthResult> CheckAsync(CancellationToken cancellationToken = default);
}
