using DETechOne.SmartWMS.Application.Common;
using DETechOne.SmartWMS.Contracts.Dtos.Audit;
using DETechOne.SmartWMS.Contracts.Requests.Audit;
using DETechOne.SmartWMS.Contracts.Responses;
using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Entities.Audit;

namespace DETechOne.SmartWMS.Application.Audit;

public sealed class AuditService : IAuditService
{
    private readonly IAuditLogRepository _repository;
    private readonly IClock _clock;

    public AuditService(IAuditLogRepository repository, IClock clock)
    {
        _repository = repository;
        _clock = clock;
    }

    public async Task<Result<AuditLogEntryDto>> WriteAsync(CreateAuditLogRequest request, string userName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Module))
        {
            return Result<AuditLogEntryDto>.Fail("AUDIT_MODULE_REQUIRED", "Audit module is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Action))
        {
            return Result<AuditLogEntryDto>.Fail("AUDIT_ACTION_REQUIRED", "Audit action is required.");
        }

        if (string.IsNullOrWhiteSpace(request.EntityType))
        {
            return Result<AuditLogEntryDto>.Fail("AUDIT_ENTITY_TYPE_REQUIRED", "Audit entity type is required.");
        }

        if (string.IsNullOrWhiteSpace(request.EntityId))
        {
            return Result<AuditLogEntryDto>.Fail("AUDIT_ENTITY_ID_REQUIRED", "Audit entity id is required.");
        }

        var entry = new AuditLogEntry(
            request.Module.Trim(),
            request.Action.Trim(),
            request.EntityType.Trim(),
            request.EntityId.Trim(),
            userName.Trim(),
            request.DeviceCode,
            request.CorrelationId,
            request.Description,
            request.Payload,
            _clock.UtcNow);

        await _repository.AddAsync(entry, cancellationToken).ConfigureAwait(false);

        return Result<AuditLogEntryDto>.Ok(AuditMapper.ToDto(entry), "Audit log registered.");
    }

    public async Task<Result<PagedResult<AuditLogEntryDto>>> SearchAsync(AuditLogQueryRequest query, CancellationToken cancellationToken = default)
    {
        var normalized = Normalize(query.Page, query.PageSize);
        var effectiveQuery = new AuditLogQueryRequest
        {
            Module = query.Module,
            Action = query.Action,
            EntityType = query.EntityType,
            EntityId = query.EntityId,
            UserName = query.UserName,
            DeviceCode = query.DeviceCode,
            CorrelationId = query.CorrelationId,
            FromUtc = query.FromUtc,
            ToUtc = query.ToUtc,
            Page = normalized.Page,
            PageSize = normalized.PageSize
        };

        var result = await _repository.SearchAsync(effectiveQuery, cancellationToken).ConfigureAwait(false);

        var page = new PagedResult<AuditLogEntryDto>
        {
            Items = result.Items.Select(AuditMapper.ToDto).ToArray(),
            Page = effectiveQuery.Page,
            PageSize = effectiveQuery.PageSize,
            TotalCount = result.TotalCount
        };

        return Result<PagedResult<AuditLogEntryDto>>.Ok(page);
    }

    private static (int Page, int PageSize) Normalize(int page, int pageSize)
    {
        int effectivePage = page <= 0 ? 1 : page;
        int effectivePageSize = pageSize <= 0 ? 50 : Math.Min(pageSize, 200);
        return (effectivePage, effectivePageSize);
    }
}
