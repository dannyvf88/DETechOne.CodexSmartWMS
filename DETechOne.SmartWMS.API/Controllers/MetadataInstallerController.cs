using DETechOne.SmartWMS.Application.Metadata;
using DETechOne.SmartWMS.Contracts.Dtos.Metadata;
using DETechOne.SmartWMS.Contracts.Requests.Metadata;
using DETechOne.SmartWMS.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DETechOne.SmartWMS.API.Controllers;

[ApiController]
[Route("api/metadata")]
[Authorize]
public sealed class MetadataInstallerController : ControllerBase
{
    private readonly IMetadataDefinitionProvider _definitionProvider;
    private readonly IMetadataInstaller _metadataInstaller;

    public MetadataInstallerController(IMetadataDefinitionProvider definitionProvider, IMetadataInstaller metadataInstaller)
    {
        _definitionProvider = definitionProvider;
        _metadataInstaller = metadataInstaller;
    }

    [HttpGet("plan")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<MetadataObjectDefinitionDto>>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<IReadOnlyList<MetadataObjectDefinitionDto>>> GetPlan()
    {
        return Ok(ApiResponse<IReadOnlyList<MetadataObjectDefinitionDto>>.Ok(_definitionProvider.GetDefinitions()));
    }

    [HttpPost("install")]
    [ProducesResponseType(typeof(ApiResponse<MetadataInstallResultDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<MetadataInstallResultDto>>> InstallAsync(
        [FromBody] RunMetadataInstallRequest request,
        CancellationToken cancellationToken)
    {
        MetadataInstallResultDto result = await _metadataInstaller.InstallAsync(request, cancellationToken).ConfigureAwait(false);

        return Ok(ApiResponse<MetadataInstallResultDto>.Ok(result));
    }
}
