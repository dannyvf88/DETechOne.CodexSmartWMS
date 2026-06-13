using DETechOne.SmartWMS.Contracts.Dtos.Picking;
using DETechOne.SmartWMS.Contracts.Requests.Picking;
using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Entities.Picking;

namespace DETechOne.SmartWMS.Application.Picking;

public sealed class PickingService : IPickingService
{
    private const string DefaultSystemUser = "system";
    private readonly IPickingRepository _pickingRepository;

    public PickingService(IPickingRepository pickingRepository)
    {
        _pickingRepository = pickingRepository;
    }

    public async Task<Result<IReadOnlyList<PickingDocumentDto>>> GetOpenAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<PickingDocument> pickings = await _pickingRepository.GetOpenAsync(cancellationToken).ConfigureAwait(false);
        return Result<IReadOnlyList<PickingDocumentDto>>.Ok(pickings.Select(PickingMapper.ToDto).ToArray());
    }

    public async Task<Result<PickingDocumentDto>> GetByIdAsync(Guid pickingId, CancellationToken cancellationToken = default)
    {
        if (pickingId == Guid.Empty)
        {
            return Result<PickingDocumentDto>.Fail("PICKING_VALIDATION", "PickingId is required.");
        }

        PickingDocument? picking = await _pickingRepository.GetByIdAsync(pickingId, cancellationToken).ConfigureAwait(false);
        if (picking is null)
        {
            return Result<PickingDocumentDto>.Fail("PICKING_NOT_FOUND", "Picking document was not found.");
        }

        return Result<PickingDocumentDto>.Ok(PickingMapper.ToDto(picking));
    }

    public async Task<Result<PickingDocumentDto>> CreateAsync(CreatePickingRequest request, string? userName, CancellationToken cancellationToken = default)
    {
        string validationError = ValidateCreateRequest(request);
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            return Result<PickingDocumentDto>.Fail("PICKING_VALIDATION", validationError);
        }

        string actor = NormalizeActor(userName);
        string pickingNumber = await _pickingRepository.GetNextPickingNumberAsync(cancellationToken).ConfigureAwait(false);

        var picking = new PickingDocument(
            pickingNumber,
            request.SourceDocumentType,
            request.SourceDocumentNumber,
            request.WarehouseCode,
            actor);

        picking.MarkCreated(actor);

        foreach (CreatePickingLineRequest lineRequest in request.Lines.OrderBy(line => line.LineNumber))
        {
            picking.AddLine(new PickingLine(
                lineRequest.LineNumber,
                lineRequest.ItemCode,
                lineRequest.WarehouseCode,
                lineRequest.LocationCode,
                lineRequest.RequiredQuantity,
                lineRequest.LotNumber,
                lineRequest.UomCode));
        }

        await _pickingRepository.AddAsync(picking, cancellationToken).ConfigureAwait(false);
        return Result<PickingDocumentDto>.Ok(PickingMapper.ToDto(picking), "Picking document created successfully.");
    }

    public async Task<Result<PickingDocumentDto>> ScanItemAsync(ScanPickingItemRequest request, string? userName, CancellationToken cancellationToken = default)
    {
        if (request.PickingId == Guid.Empty)
        {
            return Result<PickingDocumentDto>.Fail("PICKING_VALIDATION", "PickingId is required.");
        }

        if (request.LineNumber < 0)
        {
            return Result<PickingDocumentDto>.Fail("PICKING_VALIDATION", "LineNumber cannot be negative.");
        }

        if (request.Quantity <= 0)
        {
            return Result<PickingDocumentDto>.Fail("PICKING_VALIDATION", "Quantity must be greater than zero.");
        }

        PickingDocument? picking = await _pickingRepository.GetByIdAsync(request.PickingId, cancellationToken).ConfigureAwait(false);
        if (picking is null)
        {
            return Result<PickingDocumentDto>.Fail("PICKING_NOT_FOUND", "Picking document was not found.");
        }

        try
        {
            picking.ScanItem(request.LineNumber, request.Quantity, NormalizeActor(userName));
        }
        catch (InvalidOperationException exception)
        {
            return Result<PickingDocumentDto>.Fail("PICKING_INVALID_STATE", exception.Message);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            return Result<PickingDocumentDto>.Fail("PICKING_VALIDATION", exception.Message);
        }

        await _pickingRepository.UpdateAsync(picking, cancellationToken).ConfigureAwait(false);
        return Result<PickingDocumentDto>.Ok(PickingMapper.ToDto(picking), "Picking scan registered successfully.");
    }

    public async Task<Result<PickingDocumentDto>> CloseAsync(ClosePickingRequest request, string? userName, CancellationToken cancellationToken = default)
    {
        if (request.PickingId == Guid.Empty)
        {
            return Result<PickingDocumentDto>.Fail("PICKING_VALIDATION", "PickingId is required.");
        }

        PickingDocument? picking = await _pickingRepository.GetByIdAsync(request.PickingId, cancellationToken).ConfigureAwait(false);
        if (picking is null)
        {
            return Result<PickingDocumentDto>.Fail("PICKING_NOT_FOUND", "Picking document was not found.");
        }

        try
        {
            picking.Complete(NormalizeActor(userName));
        }
        catch (InvalidOperationException exception)
        {
            return Result<PickingDocumentDto>.Fail("PICKING_INVALID_STATE", exception.Message);
        }

        await _pickingRepository.UpdateAsync(picking, cancellationToken).ConfigureAwait(false);
        return Result<PickingDocumentDto>.Ok(PickingMapper.ToDto(picking), "Picking document closed successfully.");
    }

    public async Task<Result<PickingDocumentDto>> CancelAsync(CancelPickingRequest request, string? userName, CancellationToken cancellationToken = default)
    {
        if (request.PickingId == Guid.Empty)
        {
            return Result<PickingDocumentDto>.Fail("PICKING_VALIDATION", "PickingId is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return Result<PickingDocumentDto>.Fail("PICKING_VALIDATION", "Reason is required.");
        }

        PickingDocument? picking = await _pickingRepository.GetByIdAsync(request.PickingId, cancellationToken).ConfigureAwait(false);
        if (picking is null)
        {
            return Result<PickingDocumentDto>.Fail("PICKING_NOT_FOUND", "Picking document was not found.");
        }

        try
        {
            picking.Cancel(NormalizeActor(userName), request.Reason);
        }
        catch (InvalidOperationException exception)
        {
            return Result<PickingDocumentDto>.Fail("PICKING_INVALID_STATE", exception.Message);
        }

        await _pickingRepository.UpdateAsync(picking, cancellationToken).ConfigureAwait(false);
        return Result<PickingDocumentDto>.Ok(PickingMapper.ToDto(picking), "Picking document cancelled successfully.");
    }

    private static string ValidateCreateRequest(CreatePickingRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SourceDocumentType))
        {
            return "SourceDocumentType is required.";
        }

        if (string.IsNullOrWhiteSpace(request.SourceDocumentNumber))
        {
            return "SourceDocumentNumber is required.";
        }

        if (string.IsNullOrWhiteSpace(request.WarehouseCode))
        {
            return "WarehouseCode is required.";
        }

        if (request.Lines.Count == 0)
        {
            return "At least one picking line is required.";
        }

        if (request.Lines.Any(line => string.IsNullOrWhiteSpace(line.ItemCode)))
        {
            return "All picking lines require ItemCode.";
        }

        if (request.Lines.Any(line => string.IsNullOrWhiteSpace(line.WarehouseCode)))
        {
            return "All picking lines require WarehouseCode.";
        }

        if (request.Lines.Any(line => line.RequiredQuantity <= 0))
        {
            return "All picking lines require RequiredQuantity greater than zero.";
        }

        if (request.Lines.GroupBy(line => line.LineNumber).Any(group => group.Count() > 1))
        {
            return "Picking line numbers cannot be duplicated.";
        }

        return string.Empty;
    }

    private static string NormalizeActor(string? userName) => string.IsNullOrWhiteSpace(userName) ? DefaultSystemUser : userName.Trim();
}
