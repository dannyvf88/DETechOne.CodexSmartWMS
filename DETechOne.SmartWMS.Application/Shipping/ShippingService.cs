using DETechOne.SmartWMS.Contracts.Dtos.Shipping;
using DETechOne.SmartWMS.Contracts.Requests.Shipping;
using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Entities.Shipping;

namespace DETechOne.SmartWMS.Application.Shipping;

public sealed class ShippingService : IShippingService
{
    private const string DefaultSystemUser = "system";
    private readonly IShippingRepository _shippingRepository;
    private readonly ISapDeliveryService _sapDeliveryService;

    public ShippingService(IShippingRepository shippingRepository, ISapDeliveryService sapDeliveryService)
    {
        _shippingRepository = shippingRepository;
        _sapDeliveryService = sapDeliveryService;
    }

    public async Task<Result<IReadOnlyList<ShippingDocumentDto>>> GetOpenAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ShippingDocument> shippings = await _shippingRepository.GetOpenAsync(cancellationToken).ConfigureAwait(false);
        return Result<IReadOnlyList<ShippingDocumentDto>>.Ok(shippings.Select(ShippingMapper.ToDto).ToArray());
    }

    public async Task<Result<ShippingDocumentDto>> GetByIdAsync(Guid shippingId, CancellationToken cancellationToken = default)
    {
        if (shippingId == Guid.Empty)
        {
            return Result<ShippingDocumentDto>.Fail("SHIPPING_VALIDATION", "ShippingId is required.");
        }

        ShippingDocument? shipping = await _shippingRepository.GetByIdAsync(shippingId, cancellationToken).ConfigureAwait(false);
        if (shipping is null)
        {
            return Result<ShippingDocumentDto>.Fail("SHIPPING_NOT_FOUND", "Shipping document was not found.");
        }

        return Result<ShippingDocumentDto>.Ok(ShippingMapper.ToDto(shipping));
    }

    public async Task<Result<ShippingDocumentDto>> CreateAsync(CreateShippingRequest request, string? userName, CancellationToken cancellationToken = default)
    {
        string validationError = ValidateCreateRequest(request);
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            return Result<ShippingDocumentDto>.Fail("SHIPPING_VALIDATION", validationError);
        }

        string actor = NormalizeActor(userName);
        string shippingNumber = await _shippingRepository.GetNextShippingNumberAsync(cancellationToken).ConfigureAwait(false);

        var shipping = new ShippingDocument(
            shippingNumber,
            request.PackingId,
            request.PackingNumber,
            request.WarehouseCode,
            request.CustomerCode,
            request.CustomerName,
            actor);

        shipping.MarkCreated(actor);

        foreach (CreateShippingLineRequest lineRequest in request.Lines.OrderBy(line => line.LineNumber))
        {
            shipping.AddLine(new ShippingLine(
                lineRequest.LineNumber,
                lineRequest.ItemCode,
                lineRequest.WarehouseCode,
                lineRequest.LocationCode,
                lineRequest.PackedQuantity,
                lineRequest.LotNumber,
                lineRequest.UomCode));
        }

        await _shippingRepository.AddAsync(shipping, cancellationToken).ConfigureAwait(false);
        return Result<ShippingDocumentDto>.Ok(ShippingMapper.ToDto(shipping), "Shipping document created successfully.");
    }

    public async Task<Result<ShippingDocumentDto>> ConfirmAsync(ConfirmShippingRequest request, string? userName, CancellationToken cancellationToken = default)
    {
        if (request.ShippingId == Guid.Empty)
        {
            return Result<ShippingDocumentDto>.Fail("SHIPPING_VALIDATION", "ShippingId is required.");
        }

        ShippingDocument? shipping = await _shippingRepository.GetByIdAsync(request.ShippingId, cancellationToken).ConfigureAwait(false);
        if (shipping is null)
        {
            return Result<ShippingDocumentDto>.Fail("SHIPPING_NOT_FOUND", "Shipping document was not found.");
        }

        try
        {
            shipping.Confirm(NormalizeActor(userName));
        }
        catch (InvalidOperationException exception)
        {
            return Result<ShippingDocumentDto>.Fail("SHIPPING_INVALID_STATE", exception.Message);
        }

        await _shippingRepository.UpdateAsync(shipping, cancellationToken).ConfigureAwait(false);
        return Result<ShippingDocumentDto>.Ok(ShippingMapper.ToDto(shipping), "Shipping document confirmed successfully.");
    }

    public async Task<Result<ShippingDocumentDto>> CreateSapDeliveryAsync(CreateSapDeliveryRequest request, string? userName, CancellationToken cancellationToken = default)
    {
        if (request.ShippingId == Guid.Empty)
        {
            return Result<ShippingDocumentDto>.Fail("SHIPPING_VALIDATION", "ShippingId is required.");
        }

        string actor = NormalizeActor(userName);
        ShippingDocument? shipping = await _shippingRepository.GetByIdAsync(request.ShippingId, cancellationToken).ConfigureAwait(false);
        if (shipping is null)
        {
            return Result<ShippingDocumentDto>.Fail("SHIPPING_NOT_FOUND", "Shipping document was not found.");
        }

        Result<SapDeliveryResultDto> deliveryResult = await _sapDeliveryService.CreateDeliveryAsync(shipping, actor, cancellationToken).ConfigureAwait(false);
        if (!deliveryResult.Success || deliveryResult.Value is null)
        {
            return Result<ShippingDocumentDto>.Fail(deliveryResult.ErrorCode ?? "SAP_DELIVERY_ERROR", deliveryResult.Message ?? "SAP delivery could not be created.");
        }

        try
        {
            shipping.MarkDeliveryCreated(deliveryResult.Value.DocEntry, deliveryResult.Value.DocNum, actor);
        }
        catch (InvalidOperationException exception)
        {
            return Result<ShippingDocumentDto>.Fail("SHIPPING_INVALID_STATE", exception.Message);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            return Result<ShippingDocumentDto>.Fail("SHIPPING_VALIDATION", exception.Message);
        }

        await _shippingRepository.UpdateAsync(shipping, cancellationToken).ConfigureAwait(false);
        return Result<ShippingDocumentDto>.Ok(ShippingMapper.ToDto(shipping), deliveryResult.Value.Message);
    }

    public async Task<Result<ShippingDocumentDto>> CancelAsync(CancelShippingRequest request, string? userName, CancellationToken cancellationToken = default)
    {
        if (request.ShippingId == Guid.Empty)
        {
            return Result<ShippingDocumentDto>.Fail("SHIPPING_VALIDATION", "ShippingId is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return Result<ShippingDocumentDto>.Fail("SHIPPING_VALIDATION", "Reason is required.");
        }

        ShippingDocument? shipping = await _shippingRepository.GetByIdAsync(request.ShippingId, cancellationToken).ConfigureAwait(false);
        if (shipping is null)
        {
            return Result<ShippingDocumentDto>.Fail("SHIPPING_NOT_FOUND", "Shipping document was not found.");
        }

        try
        {
            shipping.Cancel(NormalizeActor(userName), request.Reason);
        }
        catch (InvalidOperationException exception)
        {
            return Result<ShippingDocumentDto>.Fail("SHIPPING_INVALID_STATE", exception.Message);
        }

        await _shippingRepository.UpdateAsync(shipping, cancellationToken).ConfigureAwait(false);
        return Result<ShippingDocumentDto>.Ok(ShippingMapper.ToDto(shipping), "Shipping document cancelled successfully.");
    }

    private static string ValidateCreateRequest(CreateShippingRequest request)
    {
        if (request.PackingId == Guid.Empty)
        {
            return "PackingId is required.";
        }

        if (string.IsNullOrWhiteSpace(request.PackingNumber))
        {
            return "PackingNumber is required.";
        }

        if (string.IsNullOrWhiteSpace(request.WarehouseCode))
        {
            return "WarehouseCode is required.";
        }

        if (string.IsNullOrWhiteSpace(request.CustomerCode))
        {
            return "CustomerCode is required.";
        }

        if (string.IsNullOrWhiteSpace(request.CustomerName))
        {
            return "CustomerName is required.";
        }

        if (request.Lines.Count == 0)
        {
            return "At least one shipping line is required.";
        }

        if (request.Lines.Any(line => string.IsNullOrWhiteSpace(line.ItemCode)))
        {
            return "All shipping lines require ItemCode.";
        }

        if (request.Lines.Any(line => string.IsNullOrWhiteSpace(line.WarehouseCode)))
        {
            return "All shipping lines require WarehouseCode.";
        }

        if (request.Lines.Any(line => line.PackedQuantity <= 0))
        {
            return "All shipping lines require PackedQuantity greater than zero.";
        }

        if (request.Lines.GroupBy(line => line.LineNumber).Any(group => group.Count() > 1))
        {
            return "Shipping line numbers cannot be duplicated.";
        }

        return string.Empty;
    }

    private static string NormalizeActor(string? userName) => string.IsNullOrWhiteSpace(userName) ? DefaultSystemUser : userName.Trim();
}
