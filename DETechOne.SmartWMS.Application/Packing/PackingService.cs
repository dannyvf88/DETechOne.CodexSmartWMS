using DETechOne.SmartWMS.Contracts.Dtos.Packing;
using DETechOne.SmartWMS.Contracts.Requests.Packing;
using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Entities.Packing;

namespace DETechOne.SmartWMS.Application.Packing;

public sealed class PackingService : IPackingService
{
    private const string DefaultSystemUser = "system";
    private readonly IPackingRepository _packingRepository;

    public PackingService(IPackingRepository packingRepository)
    {
        _packingRepository = packingRepository;
    }

    public async Task<Result<IReadOnlyList<PackingDocumentDto>>> GetOpenAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<PackingDocument> packings = await _packingRepository.GetOpenAsync(cancellationToken).ConfigureAwait(false);
        return Result<IReadOnlyList<PackingDocumentDto>>.Ok(packings.Select(PackingMapper.ToDto).ToArray());
    }

    public async Task<Result<PackingDocumentDto>> GetByIdAsync(Guid packingId, CancellationToken cancellationToken = default)
    {
        if (packingId == Guid.Empty)
        {
            return Result<PackingDocumentDto>.Fail("PACKING_VALIDATION", "PackingId is required.");
        }

        PackingDocument? packing = await _packingRepository.GetByIdAsync(packingId, cancellationToken).ConfigureAwait(false);
        if (packing is null)
        {
            return Result<PackingDocumentDto>.Fail("PACKING_NOT_FOUND", "Packing document was not found.");
        }

        return Result<PackingDocumentDto>.Ok(PackingMapper.ToDto(packing));
    }

    public async Task<Result<PackingDocumentDto>> CreateAsync(CreatePackingRequest request, string? userName, CancellationToken cancellationToken = default)
    {
        string validationError = ValidateCreateRequest(request);
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            return Result<PackingDocumentDto>.Fail("PACKING_VALIDATION", validationError);
        }

        string actor = NormalizeActor(userName);
        string packingNumber = await _packingRepository.GetNextPackingNumberAsync(cancellationToken).ConfigureAwait(false);

        var packing = new PackingDocument(
            packingNumber,
            request.PickingId,
            request.PickingNumber,
            request.WarehouseCode,
            actor);

        packing.MarkCreated(actor);

        foreach (CreatePackingLineRequest lineRequest in request.Lines.OrderBy(line => line.LineNumber))
        {
            packing.AddLine(new PackingLine(
                lineRequest.LineNumber,
                lineRequest.ItemCode,
                lineRequest.WarehouseCode,
                lineRequest.LocationCode,
                lineRequest.PickedQuantity,
                lineRequest.LotNumber,
                lineRequest.UomCode));
        }

        await _packingRepository.AddAsync(packing, cancellationToken).ConfigureAwait(false);
        return Result<PackingDocumentDto>.Ok(PackingMapper.ToDto(packing), "Packing document created successfully.");
    }

    public async Task<Result<PackingDocumentDto>> PackItemAsync(PackItemRequest request, string? userName, CancellationToken cancellationToken = default)
    {
        if (request.PackingId == Guid.Empty)
        {
            return Result<PackingDocumentDto>.Fail("PACKING_VALIDATION", "PackingId is required.");
        }

        if (request.LineNumber < 0)
        {
            return Result<PackingDocumentDto>.Fail("PACKING_VALIDATION", "LineNumber cannot be negative.");
        }

        if (request.Quantity <= 0)
        {
            return Result<PackingDocumentDto>.Fail("PACKING_VALIDATION", "Quantity must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(request.PackageCode))
        {
            return Result<PackingDocumentDto>.Fail("PACKING_VALIDATION", "PackageCode is required.");
        }

        PackingDocument? packing = await _packingRepository.GetByIdAsync(request.PackingId, cancellationToken).ConfigureAwait(false);
        if (packing is null)
        {
            return Result<PackingDocumentDto>.Fail("PACKING_NOT_FOUND", "Packing document was not found.");
        }

        try
        {
            packing.PackItem(request.LineNumber, request.Quantity, request.PackageCode, NormalizeActor(userName));
        }
        catch (InvalidOperationException exception)
        {
            return Result<PackingDocumentDto>.Fail("PACKING_INVALID_STATE", exception.Message);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            return Result<PackingDocumentDto>.Fail("PACKING_VALIDATION", exception.Message);
        }

        await _packingRepository.UpdateAsync(packing, cancellationToken).ConfigureAwait(false);
        return Result<PackingDocumentDto>.Ok(PackingMapper.ToDto(packing), "Packing scan registered successfully.");
    }

    public async Task<Result<PackingDocumentDto>> CloseAsync(ClosePackingRequest request, string? userName, CancellationToken cancellationToken = default)
    {
        if (request.PackingId == Guid.Empty)
        {
            return Result<PackingDocumentDto>.Fail("PACKING_VALIDATION", "PackingId is required.");
        }

        PackingDocument? packing = await _packingRepository.GetByIdAsync(request.PackingId, cancellationToken).ConfigureAwait(false);
        if (packing is null)
        {
            return Result<PackingDocumentDto>.Fail("PACKING_NOT_FOUND", "Packing document was not found.");
        }

        try
        {
            packing.Complete(NormalizeActor(userName));
        }
        catch (InvalidOperationException exception)
        {
            return Result<PackingDocumentDto>.Fail("PACKING_INVALID_STATE", exception.Message);
        }

        await _packingRepository.UpdateAsync(packing, cancellationToken).ConfigureAwait(false);
        return Result<PackingDocumentDto>.Ok(PackingMapper.ToDto(packing), "Packing document closed successfully.");
    }

    public async Task<Result<PackingDocumentDto>> CancelAsync(CancelPackingRequest request, string? userName, CancellationToken cancellationToken = default)
    {
        if (request.PackingId == Guid.Empty)
        {
            return Result<PackingDocumentDto>.Fail("PACKING_VALIDATION", "PackingId is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return Result<PackingDocumentDto>.Fail("PACKING_VALIDATION", "Reason is required.");
        }

        PackingDocument? packing = await _packingRepository.GetByIdAsync(request.PackingId, cancellationToken).ConfigureAwait(false);
        if (packing is null)
        {
            return Result<PackingDocumentDto>.Fail("PACKING_NOT_FOUND", "Packing document was not found.");
        }

        try
        {
            packing.Cancel(NormalizeActor(userName), request.Reason);
        }
        catch (InvalidOperationException exception)
        {
            return Result<PackingDocumentDto>.Fail("PACKING_INVALID_STATE", exception.Message);
        }

        await _packingRepository.UpdateAsync(packing, cancellationToken).ConfigureAwait(false);
        return Result<PackingDocumentDto>.Ok(PackingMapper.ToDto(packing), "Packing document cancelled successfully.");
    }

    private static string ValidateCreateRequest(CreatePackingRequest request)
    {
        if (request.PickingId == Guid.Empty)
        {
            return "PickingId is required.";
        }

        if (string.IsNullOrWhiteSpace(request.PickingNumber))
        {
            return "PickingNumber is required.";
        }

        if (string.IsNullOrWhiteSpace(request.WarehouseCode))
        {
            return "WarehouseCode is required.";
        }

        if (request.Lines.Count == 0)
        {
            return "At least one packing line is required.";
        }

        if (request.Lines.Any(line => string.IsNullOrWhiteSpace(line.ItemCode)))
        {
            return "All packing lines require ItemCode.";
        }

        if (request.Lines.Any(line => string.IsNullOrWhiteSpace(line.WarehouseCode)))
        {
            return "All packing lines require WarehouseCode.";
        }

        if (request.Lines.Any(line => line.PickedQuantity <= 0))
        {
            return "All packing lines require PickedQuantity greater than zero.";
        }

        if (request.Lines.GroupBy(line => line.LineNumber).Any(group => group.Count() > 1))
        {
            return "Packing line numbers cannot be duplicated.";
        }

        return string.Empty;
    }

    private static string NormalizeActor(string? userName) => string.IsNullOrWhiteSpace(userName) ? DefaultSystemUser : userName.Trim();
}
