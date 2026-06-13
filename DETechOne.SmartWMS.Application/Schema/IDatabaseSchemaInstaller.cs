using DETechOne.SmartWMS.Contracts.Dtos.Schema;
using DETechOne.SmartWMS.Contracts.Requests.Schema;

namespace DETechOne.SmartWMS.Application.Schema;

public interface IDatabaseSchemaInstaller
{
    Task<SchemaInstallResultDto> InstallAsync(RunSchemaInstallRequest request, CancellationToken cancellationToken = default);
}
