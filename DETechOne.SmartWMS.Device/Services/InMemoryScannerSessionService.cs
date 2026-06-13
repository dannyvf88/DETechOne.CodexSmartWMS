using System.Collections.Concurrent;
using DETechOne.SmartWMS.Contracts.Dtos.Device;
using DETechOne.SmartWMS.Contracts.Requests.Device;
using DETechOne.SmartWMS.Device.Mapping;
using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Entities.Device;

namespace DETechOne.SmartWMS.Device.Services;

public sealed class InMemoryScannerSessionService : IScannerSessionService
{
    private static readonly ConcurrentDictionary<Guid, ScannerSession> Sessions = new();

    public Task<Result<ScannerSessionDto>> StartAsync(StartScannerSessionRequest request, string userName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(request.DeviceCode))
        {
            return Task.FromResult(Result<ScannerSessionDto>.Fail("DEVICE_CODE_REQUIRED", "Device code is required."));
        }

        if (string.IsNullOrWhiteSpace(request.Operation))
        {
            return Task.FromResult(Result<ScannerSessionDto>.Fail("OPERATION_REQUIRED", "Scanner operation is required."));
        }

        var session = new ScannerSession(
            request.DeviceCode,
            request.Operation,
            request.ReferenceDocument,
            userName);

        Sessions[session.Id] = session;

        return Task.FromResult(Result<ScannerSessionDto>.Ok(DeviceMapper.ToDto(session), "Scanner session started."));
    }

    public Task<Result<ScannerEventDto>> SubmitScanAsync(SubmitScanRequest request, string userName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (request.ScannerSessionId == Guid.Empty)
        {
            return Task.FromResult(Result<ScannerEventDto>.Fail("SCANNER_SESSION_REQUIRED", "Scanner session id is required."));
        }

        if (string.IsNullOrWhiteSpace(request.Value))
        {
            return Task.FromResult(Result<ScannerEventDto>.Fail("SCAN_VALUE_REQUIRED", "Scan value is required."));
        }

        if (!Sessions.TryGetValue(request.ScannerSessionId, out var session))
        {
            return Task.FromResult(Result<ScannerEventDto>.Fail("SCANNER_SESSION_NOT_FOUND", "Scanner session was not found."));
        }

        try
        {
            var scannerEvent = session.AddEvent(request.EventType, request.Value, request.Symbology, request.Source);
            return Task.FromResult(Result<ScannerEventDto>.Ok(DeviceMapper.ToDto(scannerEvent), "Scan registered."));
        }
        catch (InvalidOperationException ex)
        {
            return Task.FromResult(Result<ScannerEventDto>.Fail("SCANNER_SESSION_NOT_OPEN", ex.Message));
        }
    }

    public Task<Result<ScannerSessionDto>> CompleteAsync(CompleteScannerSessionRequest request, string userName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (request.ScannerSessionId == Guid.Empty)
        {
            return Task.FromResult(Result<ScannerSessionDto>.Fail("SCANNER_SESSION_REQUIRED", "Scanner session id is required."));
        }

        if (!Sessions.TryGetValue(request.ScannerSessionId, out var session))
        {
            return Task.FromResult(Result<ScannerSessionDto>.Fail("SCANNER_SESSION_NOT_FOUND", "Scanner session was not found."));
        }

        try
        {
            session.Complete(userName);
            return Task.FromResult(Result<ScannerSessionDto>.Ok(DeviceMapper.ToDto(session), "Scanner session completed."));
        }
        catch (InvalidOperationException ex)
        {
            return Task.FromResult(Result<ScannerSessionDto>.Fail("SCANNER_SESSION_INVALID_STATUS", ex.Message));
        }
    }

    public Task<Result<ScannerSessionDto>> CancelAsync(Guid scannerSessionId, string reason, string userName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (scannerSessionId == Guid.Empty)
        {
            return Task.FromResult(Result<ScannerSessionDto>.Fail("SCANNER_SESSION_REQUIRED", "Scanner session id is required."));
        }

        if (!Sessions.TryGetValue(scannerSessionId, out var session))
        {
            return Task.FromResult(Result<ScannerSessionDto>.Fail("SCANNER_SESSION_NOT_FOUND", "Scanner session was not found."));
        }

        try
        {
            session.Cancel(reason, userName);
            return Task.FromResult(Result<ScannerSessionDto>.Ok(DeviceMapper.ToDto(session), "Scanner session cancelled."));
        }
        catch (InvalidOperationException ex)
        {
            return Task.FromResult(Result<ScannerSessionDto>.Fail("SCANNER_SESSION_INVALID_STATUS", ex.Message));
        }
    }

    public Task<Result<ScannerSessionDto>> GetAsync(Guid scannerSessionId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!Sessions.TryGetValue(scannerSessionId, out var session))
        {
            return Task.FromResult(Result<ScannerSessionDto>.Fail("SCANNER_SESSION_NOT_FOUND", "Scanner session was not found."));
        }

        return Task.FromResult(Result<ScannerSessionDto>.Ok(DeviceMapper.ToDto(session)));
    }
}
