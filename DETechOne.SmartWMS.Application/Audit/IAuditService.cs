using DETechOne.SmartWMS.Contracts.Dtos.Audit;
using DETechOne.SmartWMS.Contracts.Requests.Audit;
using DETechOne.SmartWMS.Contracts.Responses;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.Audit;

public interface IAuditService
{
    Task<Result<AuditLogEntryDto>> WriteAsync(CreateAuditLogRequest request, string userName, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<AuditLogEntryDto>>> SearchAsync(AuditLogQueryRequest query, CancellationToken cancellationToken = default);
}
