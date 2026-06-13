using DETechOne.SmartWMS.Application.Security;
using DETechOne.SmartWMS.Contracts.Dtos.Device;
using DETechOne.SmartWMS.Contracts.Requests.Device;
using DETechOne.SmartWMS.Contracts.Responses;
using DETechOne.SmartWMS.Device.Services;
using Microsoft.AspNetCore.Mvc;

namespace DETechOne.SmartWMS.API.Controllers;

[ApiController]
[Route("api/devices")]
public sealed class DeviceController : ControllerBase
{
    private readonly IDeviceRegistryService _deviceRegistryService;
    private readonly IScannerSessionService _scannerSessionService;
    private readonly IUserContextService _userContextService;

    public DeviceController(
        IDeviceRegistryService deviceRegistryService,
        IScannerSessionService scannerSessionService,
        IUserContextService userContextService)
    {
        _deviceRegistryService = deviceRegistryService;
        _scannerSessionService = scannerSessionService;
        _userContextService = userContextService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<DeviceRegistrationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<DeviceRegistrationDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<DeviceRegistrationDto>>> RegisterDevice(
        RegisterDeviceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _deviceRegistryService
            .RegisterAsync(request, ResolveUserName(), cancellationToken)
            .ConfigureAwait(false);

        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<DeviceRegistrationDto>.Fail(
                result.ErrorCode ?? "DEVICE_REGISTER_ERROR",
                result.Message ?? "Device could not be registered."));
        }

        return Ok(ApiResponse<DeviceRegistrationDto>.Ok(result.Value, result.Message));
    }

    [HttpPost("heartbeat")]
    [ProducesResponseType(typeof(ApiResponse<DeviceHeartbeatDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<DeviceHeartbeatDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<DeviceHeartbeatDto>>> Heartbeat(
        DeviceHeartbeatRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _deviceRegistryService
            .HeartbeatAsync(request, ResolveUserName(), cancellationToken)
            .ConfigureAwait(false);

        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<DeviceHeartbeatDto>.Fail(
                result.ErrorCode ?? "DEVICE_HEARTBEAT_ERROR",
                result.Message ?? "Device heartbeat could not be registered."));
        }

        return Ok(ApiResponse<DeviceHeartbeatDto>.Ok(result.Value, result.Message));
    }

    [HttpGet("{deviceCode}")]
    [ProducesResponseType(typeof(ApiResponse<DeviceRegistrationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<DeviceRegistrationDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<DeviceRegistrationDto>>> GetDevice(
        string deviceCode,
        CancellationToken cancellationToken)
    {
        var result = await _deviceRegistryService.GetAsync(deviceCode, cancellationToken).ConfigureAwait(false);

        if (!result.Success || result.Value is null)
        {
            return NotFound(ApiResponse<DeviceRegistrationDto>.Fail(
                result.ErrorCode ?? "DEVICE_NOT_FOUND",
                result.Message ?? "Device was not found."));
        }

        return Ok(ApiResponse<DeviceRegistrationDto>.Ok(result.Value, result.Message));
    }

    [HttpGet("online")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<DeviceRegistrationDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<DeviceRegistrationDto>>>> GetOnlineDevices(
        CancellationToken cancellationToken)
    {
        var result = await _deviceRegistryService.GetOnlineAsync(cancellationToken).ConfigureAwait(false);
        return Ok(ApiResponse<IReadOnlyCollection<DeviceRegistrationDto>>.Ok(
            result.Value ?? Array.Empty<DeviceRegistrationDto>(),
            result.Message));
    }

    [HttpPost("scanner-sessions/start")]
    [ProducesResponseType(typeof(ApiResponse<ScannerSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ScannerSessionDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ScannerSessionDto>>> StartScannerSession(
        StartScannerSessionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _scannerSessionService
            .StartAsync(request, ResolveUserName(), cancellationToken)
            .ConfigureAwait(false);

        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<ScannerSessionDto>.Fail(
                result.ErrorCode ?? "SCANNER_SESSION_START_ERROR",
                result.Message ?? "Scanner session could not be started."));
        }

        return Ok(ApiResponse<ScannerSessionDto>.Ok(result.Value, result.Message));
    }

    [HttpPost("scanner-sessions/scan")]
    [ProducesResponseType(typeof(ApiResponse<ScannerEventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ScannerEventDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ScannerEventDto>>> SubmitScan(
        SubmitScanRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _scannerSessionService
            .SubmitScanAsync(request, ResolveUserName(), cancellationToken)
            .ConfigureAwait(false);

        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<ScannerEventDto>.Fail(
                result.ErrorCode ?? "SCAN_REGISTER_ERROR",
                result.Message ?? "Scan could not be registered."));
        }

        return Ok(ApiResponse<ScannerEventDto>.Ok(result.Value, result.Message));
    }

    [HttpPost("scanner-sessions/complete")]
    [ProducesResponseType(typeof(ApiResponse<ScannerSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ScannerSessionDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ScannerSessionDto>>> CompleteScannerSession(
        CompleteScannerSessionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _scannerSessionService
            .CompleteAsync(request, ResolveUserName(), cancellationToken)
            .ConfigureAwait(false);

        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<ScannerSessionDto>.Fail(
                result.ErrorCode ?? "SCANNER_SESSION_COMPLETE_ERROR",
                result.Message ?? "Scanner session could not be completed."));
        }

        return Ok(ApiResponse<ScannerSessionDto>.Ok(result.Value, result.Message));
    }

    [HttpPost("scanner-sessions/{scannerSessionId:guid}/cancel")]
    [ProducesResponseType(typeof(ApiResponse<ScannerSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ScannerSessionDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ScannerSessionDto>>> CancelScannerSession(
        Guid scannerSessionId,
        CancelScannerSessionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _scannerSessionService
            .CancelAsync(scannerSessionId, request.Reason, ResolveUserName(), cancellationToken)
            .ConfigureAwait(false);

        if (!result.Success || result.Value is null)
        {
            return BadRequest(ApiResponse<ScannerSessionDto>.Fail(
                result.ErrorCode ?? "SCANNER_SESSION_CANCEL_ERROR",
                result.Message ?? "Scanner session could not be cancelled."));
        }

        return Ok(ApiResponse<ScannerSessionDto>.Ok(result.Value, result.Message));
    }

    [HttpGet("scanner-sessions/{scannerSessionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ScannerSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ScannerSessionDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ScannerSessionDto>>> GetScannerSession(
        Guid scannerSessionId,
        CancellationToken cancellationToken)
    {
        var result = await _scannerSessionService.GetAsync(scannerSessionId, cancellationToken).ConfigureAwait(false);

        if (!result.Success || result.Value is null)
        {
            return NotFound(ApiResponse<ScannerSessionDto>.Fail(
                result.ErrorCode ?? "SCANNER_SESSION_NOT_FOUND",
                result.Message ?? "Scanner session was not found."));
        }

        return Ok(ApiResponse<ScannerSessionDto>.Ok(result.Value, result.Message));
    }

    private string ResolveUserName()
    {
        return string.IsNullOrWhiteSpace(_userContextService.UserName)
            ? "SmartWMS.Device"
            : _userContextService.UserName;
    }
}
