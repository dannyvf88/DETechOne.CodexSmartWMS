using DETechOne.SmartWMS.Application.Common;
using DETechOne.SmartWMS.Contracts.Dtos.Alerts;
using DETechOne.SmartWMS.Contracts.Requests.Alerts;
using DETechOne.SmartWMS.Contracts.Responses;
using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Entities.Alerts;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Application.Alerts;

public sealed class AlertService : IAlertService
{
    private readonly IAlertRepository _repository;
    private readonly IClock _clock;

    public AlertService(IAlertRepository repository, IClock clock)
    {
        _repository = repository;
        _clock = clock;
    }

    public async Task<Result<OperationalAlertDto>> CreateAsync(CreateOperationalAlertRequest request, string userName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Source))
        {
            return Result<OperationalAlertDto>.Fail("ALERT_SOURCE_REQUIRED", "Alert source is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return Result<OperationalAlertDto>.Fail("ALERT_CODE_REQUIRED", "Alert code is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return Result<OperationalAlertDto>.Fail("ALERT_TITLE_REQUIRED", "Alert title is required.");
        }

        if (!Enum.TryParse(request.Severity, true, out AlertSeverity severity))
        {
            severity = AlertSeverity.Warning;
        }

        var alert = new OperationalAlert(
            severity,
            AlertStatus.Open,
            request.Source.Trim(),
            request.Code.Trim(),
            request.Title.Trim(),
            request.Message,
            request.EntityType,
            request.EntityId,
            userName,
            request.DeviceCode,
            request.CorrelationId,
            _clock.UtcNow);

        await _repository.AddAsync(alert, cancellationToken).ConfigureAwait(false);

        return Result<OperationalAlertDto>.Ok(AlertMapper.ToDto(alert), "Operational alert created.");
    }

    public async Task<Result<PagedResult<OperationalAlertDto>>> SearchAsync(AlertQueryRequest query, CancellationToken cancellationToken = default)
    {
        var normalized = Normalize(query.Page, query.PageSize);
        var effectiveQuery = new AlertQueryRequest
        {
            Severity = query.Severity,
            Status = query.Status,
            Source = query.Source,
            Code = query.Code,
            EntityType = query.EntityType,
            EntityId = query.EntityId,
            DeviceCode = query.DeviceCode,
            FromUtc = query.FromUtc,
            ToUtc = query.ToUtc,
            Page = normalized.Page,
            PageSize = normalized.PageSize
        };

        var result = await _repository.SearchAsync(effectiveQuery, cancellationToken).ConfigureAwait(false);

        var page = new PagedResult<OperationalAlertDto>
        {
            Items = result.Items.Select(AlertMapper.ToDto).ToArray(),
            Page = effectiveQuery.Page,
            PageSize = effectiveQuery.PageSize,
            TotalCount = result.TotalCount
        };

        return Result<PagedResult<OperationalAlertDto>>.Ok(page);
    }

    public async Task<Result<OperationalAlertDto>> AcknowledgeAsync(Guid alertId, string userName, CancellationToken cancellationToken = default)
    {
        var alert = await _repository.GetByIdAsync(alertId, cancellationToken).ConfigureAwait(false);
        if (alert is null)
        {
            return Result<OperationalAlertDto>.Fail("ALERT_NOT_FOUND", "Operational alert was not found.");
        }

        alert.Acknowledge(userName, _clock.UtcNow);
        await _repository.UpdateAsync(alert, cancellationToken).ConfigureAwait(false);

        return Result<OperationalAlertDto>.Ok(AlertMapper.ToDto(alert), "Operational alert acknowledged.");
    }

    public async Task<Result<OperationalAlertDto>> ResolveAsync(Guid alertId, string userName, CancellationToken cancellationToken = default)
    {
        var alert = await _repository.GetByIdAsync(alertId, cancellationToken).ConfigureAwait(false);
        if (alert is null)
        {
            return Result<OperationalAlertDto>.Fail("ALERT_NOT_FOUND", "Operational alert was not found.");
        }

        alert.Resolve(userName, _clock.UtcNow);
        await _repository.UpdateAsync(alert, cancellationToken).ConfigureAwait(false);

        return Result<OperationalAlertDto>.Ok(AlertMapper.ToDto(alert), "Operational alert resolved.");
    }

    private static (int Page, int PageSize) Normalize(int page, int pageSize)
    {
        int effectivePage = page <= 0 ? 1 : page;
        int effectivePageSize = pageSize <= 0 ? 50 : Math.Min(pageSize, 200);
        return (effectivePage, effectivePageSize);
    }
}
