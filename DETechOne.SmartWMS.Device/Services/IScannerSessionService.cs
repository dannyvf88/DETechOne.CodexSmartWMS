using DETechOne.SmartWMS.Contracts.Dtos.Device;
using DETechOne.SmartWMS.Contracts.Requests.Device;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Device.Services;

public interface IScannerSessionService
{
    Task<Result<ScannerSessionDto>> StartAsync(StartScannerSessionRequest request, string userName, CancellationToken cancellationToken = default);
    Task<Result<ScannerEventDto>> SubmitScanAsync(SubmitScanRequest request, string userName, CancellationToken cancellationToken = default);
    Task<Result<ScannerSessionDto>> CompleteAsync(CompleteScannerSessionRequest request, string userName, CancellationToken cancellationToken = default);
    Task<Result<ScannerSessionDto>> CancelAsync(Guid scannerSessionId, string reason, string userName, CancellationToken cancellationToken = default);
    Task<Result<ScannerSessionDto>> GetAsync(Guid scannerSessionId, CancellationToken cancellationToken = default);
}
