using DETechOne.SmartWMS.Contracts.Dtos.Metadata;
using DETechOne.SmartWMS.Contracts.Requests.Metadata;

namespace DETechOne.SmartWMS.Application.Metadata;

public interface IMetadataInstaller
{
    Task<MetadataInstallResultDto> InstallAsync(RunMetadataInstallRequest request, CancellationToken cancellationToken = default);
}
