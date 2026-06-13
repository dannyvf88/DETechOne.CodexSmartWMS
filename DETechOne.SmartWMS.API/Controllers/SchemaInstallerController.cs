using DETechOne.SmartWMS.Application.Schema;
using DETechOne.SmartWMS.Contracts.Dtos.Schema;
using DETechOne.SmartWMS.Contracts.Requests.Schema;
using DETechOne.SmartWMS.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DETechOne.SmartWMS.API.Controllers;

[ApiController]
[Route("api/schema")]
[Authorize]
public sealed class SchemaInstallerController : ControllerBase
{
    private readonly IDatabaseSchemaDefinitionProvider _definitionProvider;
    private readonly IDatabaseSchemaInstaller _schemaInstaller;

    public SchemaInstallerController(
        IDatabaseSchemaDefinitionProvider definitionProvider,
        IDatabaseSchemaInstaller schemaInstaller)
    {
        _definitionProvider = definitionProvider;
        _schemaInstaller = schemaInstaller;
    }

    [HttpGet("plan")]
    [ProducesResponseType(typeof(ApiResponse<SchemaInstallPlanDto>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<SchemaInstallPlanDto>> GetPlan()
    {
        return Ok(ApiResponse<SchemaInstallPlanDto>.Ok(_definitionProvider.GetPlan()));
    }

    [HttpPost("install")]
    [ProducesResponseType(typeof(ApiResponse<SchemaInstallResultDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<SchemaInstallResultDto>>> InstallAsync(
        [FromBody] RunSchemaInstallRequest request,
        CancellationToken cancellationToken)
    {
        SchemaInstallResultDto result = await _schemaInstaller.InstallAsync(request, cancellationToken).ConfigureAwait(false);
        return Ok(ApiResponse<SchemaInstallResultDto>.Ok(result));
    }
}
