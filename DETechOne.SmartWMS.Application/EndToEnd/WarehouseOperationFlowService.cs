using DETechOne.SmartWMS.Application.Packing;
using DETechOne.SmartWMS.Application.Picking;
using DETechOne.SmartWMS.Application.Shipping;
using DETechOne.SmartWMS.Contracts.Dtos.Packing;
using DETechOne.SmartWMS.Contracts.Dtos.Picking;
using DETechOne.SmartWMS.Contracts.Dtos.Shipping;
using DETechOne.SmartWMS.Contracts.Requests.Packing;
using DETechOne.SmartWMS.Contracts.Requests.Picking;
using DETechOne.SmartWMS.Contracts.Requests.Shipping;
using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Application.EndToEnd;

/// <summary>
/// Real integration service for the warehouse operational engines used by Order-to-Delivery flows.
/// This service is intentionally placed in Application so it can orchestrate existing Picking,
/// Packing and Shipping application services without introducing infrastructure dependencies.
/// </summary>
public sealed class WarehouseOperationFlowService : IWarehouseOperationFlowService
{
    private const string DefaultPackageCode = "PKG-DEFAULT";

    private readonly IPickingService _pickingService;
    private readonly IPackingService _packingService;
    private readonly IShippingService _shippingService;

    public WarehouseOperationFlowService(
        IPickingService pickingService,
        IPackingService packingService,
        IShippingService shippingService)
    {
        _pickingService = pickingService;
        _packingService = packingService;
        _shippingService = shippingService;
    }

    public async Task<Result<PickingDocumentDto>> CompletePickingAsync(
        Guid pickingId,
        bool autoCompletePicking,
        string userName,
        CancellationToken cancellationToken = default)
    {
        if (pickingId == Guid.Empty)
        {
            return Result<PickingDocumentDto>.Fail("PICKING_VALIDATION", "PickingId is required.");
        }

        if (string.IsNullOrWhiteSpace(userName))
        {
            return Result<PickingDocumentDto>.Fail("PICKING_VALIDATION", "UserName is required.");
        }

        Result<PickingDocumentDto> pickingResult = await _pickingService
            .GetByIdAsync(pickingId, cancellationToken)
            .ConfigureAwait(false);

        if (!pickingResult.Success || pickingResult.Value is null)
        {
            return Result<PickingDocumentDto>.Fail(
                pickingResult.ErrorCode ?? "PICKING_NOT_FOUND",
                pickingResult.Message ?? "Picking document was not found.");
        }

        PickingDocumentDto picking = pickingResult.Value;

        if (IsCompletedStatus(picking.Status))
        {
            return Result<PickingDocumentDto>.Ok(picking, "Picking is already completed.");
        }

        if (picking.PendingQuantity > 0 && !autoCompletePicking)
        {
            return Result<PickingDocumentDto>.Fail(
                "PICKING_PENDING_QUANTITY",
                "Picking has pending quantities. Complete scans or enable AutoCompletePicking.");
        }

        if (picking.PendingQuantity > 0)
        {
            foreach (PickingLineDto line in picking.Lines
                .Where(line => line.PendingQuantity > 0)
                .OrderBy(line => line.LineNumber))
            {
                Result<PickingDocumentDto> scanResult = await _pickingService
                    .ScanItemAsync(new ScanPickingItemRequest
                    {
                        PickingId = picking.Id,
                        LineNumber = line.LineNumber,
                        ItemCode = line.ItemCode,
                        Quantity = line.PendingQuantity,
                        LotNumber = line.LotNumber
                    }, userName, cancellationToken)
                    .ConfigureAwait(false);

                if (!scanResult.Success || scanResult.Value is null)
                {
                    return Result<PickingDocumentDto>.Fail(
                        scanResult.ErrorCode ?? "PICKING_SCAN_ERROR",
                        scanResult.Message ?? "Picking scan could not be registered.");
                }

                picking = scanResult.Value;
            }
        }

        if (!IsCompletedStatus(picking.Status))
        {
            Result<PickingDocumentDto> closeResult = await _pickingService
                .CloseAsync(new ClosePickingRequest
                {
                    PickingId = picking.Id
                }, userName, cancellationToken)
                .ConfigureAwait(false);

            if (!closeResult.Success || closeResult.Value is null)
            {
                return Result<PickingDocumentDto>.Fail(
                    closeResult.ErrorCode ?? "PICKING_CLOSE_ERROR",
                    closeResult.Message ?? "Picking document could not be closed.");
            }

            picking = closeResult.Value;
        }

        return Result<PickingDocumentDto>.Ok(picking, "Picking completed successfully.");
    }

    public async Task<Result<PackingDocumentDto>> CreatePackingFromPickingAsync(
        PickingDocumentDto picking,
        string userName,
        CancellationToken cancellationToken = default)
    {
        if (picking.Id == Guid.Empty)
        {
            return Result<PackingDocumentDto>.Fail("PACKING_VALIDATION", "PickingId is required.");
        }

        if (!IsCompletedStatus(picking.Status) && picking.PendingQuantity > 0)
        {
            return Result<PackingDocumentDto>.Fail(
                "PACKING_PICKING_NOT_COMPLETED",
                "Packing cannot be created until picking is completed.");
        }

        var request = new CreatePackingRequest
        {
            PickingId = picking.Id,
            PickingNumber = picking.PickingNumber,
            WarehouseCode = picking.WarehouseCode,
            Lines = picking.Lines
                .Where(line => line.PickedQuantity > 0)
                .OrderBy(line => line.LineNumber)
                .Select(line => new CreatePackingLineRequest
                {
                    LineNumber = line.LineNumber,
                    ItemCode = line.ItemCode,
                    WarehouseCode = line.WarehouseCode,
                    LocationCode = line.LocationCode,
                    PickedQuantity = line.PickedQuantity,
                    LotNumber = line.LotNumber,
                    UomCode = line.UomCode
                })
                .ToList()
        };

        if (request.Lines.Count == 0)
        {
            return Result<PackingDocumentDto>.Fail(
                "PACKING_NO_LINES",
                "Picking document does not contain picked quantities available for packing.");
        }

        return await _packingService
            .CreateAsync(request, userName, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Result<PackingDocumentDto>> CompletePackingAsync(
        Guid packingId,
        bool autoCompletePacking,
        string? packageCode,
        string userName,
        CancellationToken cancellationToken = default)
    {
        if (packingId == Guid.Empty)
        {
            return Result<PackingDocumentDto>.Fail("PACKING_VALIDATION", "PackingId is required.");
        }

        Result<PackingDocumentDto> packingResult = await _packingService
            .GetByIdAsync(packingId, cancellationToken)
            .ConfigureAwait(false);

        if (!packingResult.Success || packingResult.Value is null)
        {
            return Result<PackingDocumentDto>.Fail(
                packingResult.ErrorCode ?? "PACKING_NOT_FOUND",
                packingResult.Message ?? "Packing document was not found.");
        }

        PackingDocumentDto packing = packingResult.Value;

        if (packing.Status == PackingStatus.Completed)
        {
            return Result<PackingDocumentDto>.Ok(packing, "Packing is already completed.");
        }

        if (packing.PendingQuantity > 0 && !autoCompletePacking)
        {
            return Result<PackingDocumentDto>.Fail(
                "PACKING_PENDING_QUANTITY",
                "Packing has pending quantities. Pack items or enable AutoCompletePacking.");
        }

        if (packing.PendingQuantity > 0)
        {
            string resolvedPackageCode = string.IsNullOrWhiteSpace(packageCode)
                ? DefaultPackageCode
                : packageCode.Trim();

            foreach (PackingLineDto line in packing.Lines
                .Where(line => line.PendingQuantity > 0)
                .OrderBy(line => line.LineNumber))
            {
                Result<PackingDocumentDto> packResult = await _packingService
                    .PackItemAsync(new PackItemRequest
                    {
                        PackingId = packing.Id,
                        LineNumber = line.LineNumber,
                        Quantity = line.PendingQuantity,
                        PackageCode = resolvedPackageCode
                    }, userName, cancellationToken)
                    .ConfigureAwait(false);

                if (!packResult.Success || packResult.Value is null)
                {
                    return Result<PackingDocumentDto>.Fail(
                        packResult.ErrorCode ?? "PACKING_PACK_ERROR",
                        packResult.Message ?? "Packing item could not be registered.");
                }

                packing = packResult.Value;
            }
        }

        if (packing.Status != PackingStatus.Completed)
        {
            Result<PackingDocumentDto> closeResult = await _packingService
                .CloseAsync(new ClosePackingRequest
                {
                    PackingId = packing.Id
                }, userName, cancellationToken)
                .ConfigureAwait(false);

            if (!closeResult.Success || closeResult.Value is null)
            {
                return Result<PackingDocumentDto>.Fail(
                    closeResult.ErrorCode ?? "PACKING_CLOSE_ERROR",
                    closeResult.Message ?? "Packing document could not be closed.");
            }

            packing = closeResult.Value;
        }

        return Result<PackingDocumentDto>.Ok(packing, "Packing completed successfully.");
    }

    public async Task<Result<ShippingDocumentDto>> CreateShippingFromPackingAsync(
        PackingDocumentDto packing,
        string customerCode,
        string customerName,
        string userName,
        CancellationToken cancellationToken = default)
    {
        if (packing.Id == Guid.Empty)
        {
            return Result<ShippingDocumentDto>.Fail("SHIPPING_VALIDATION", "PackingId is required.");
        }

        if (packing.Status != PackingStatus.Completed && packing.PendingQuantity > 0)
        {
            return Result<ShippingDocumentDto>.Fail(
                "SHIPPING_PACKING_NOT_COMPLETED",
                "Shipping cannot be created until packing is completed.");
        }

        var request = new CreateShippingRequest
        {
            PackingId = packing.Id,
            PackingNumber = packing.PackingNumber,
            WarehouseCode = packing.WarehouseCode,
            CustomerCode = customerCode,
            CustomerName = customerName,
            Lines = packing.Lines
                .Where(line => line.PackedQuantity > 0)
                .OrderBy(line => line.LineNumber)
                .Select(line => new CreateShippingLineRequest
                {
                    LineNumber = line.LineNumber,
                    ItemCode = line.ItemCode,
                    WarehouseCode = line.WarehouseCode,
                    LocationCode = line.LocationCode,
                    PackedQuantity = line.PackedQuantity,
                    LotNumber = line.LotNumber,
                    UomCode = line.UomCode
                })
                .ToArray()
        };

        if (request.Lines.Count == 0)
        {
            return Result<ShippingDocumentDto>.Fail(
                "SHIPPING_NO_LINES",
                "Packing document does not contain packed quantities available for shipping.");
        }

        return await _shippingService
            .CreateAsync(request, userName, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Result<ShippingDocumentDto>> ConfirmShippingAsync(
        Guid shippingId,
        bool autoConfirmShipping,
        string userName,
        CancellationToken cancellationToken = default)
    {
        if (shippingId == Guid.Empty)
        {
            return Result<ShippingDocumentDto>.Fail("SHIPPING_VALIDATION", "ShippingId is required.");
        }

        Result<ShippingDocumentDto> shippingResult = await _shippingService
            .GetByIdAsync(shippingId, cancellationToken)
            .ConfigureAwait(false);

        if (!shippingResult.Success || shippingResult.Value is null)
        {
            return Result<ShippingDocumentDto>.Fail(
                shippingResult.ErrorCode ?? "SHIPPING_NOT_FOUND",
                shippingResult.Message ?? "Shipping document was not found.");
        }

        ShippingDocumentDto shipping = shippingResult.Value;

        if (IsConfirmedStatus(shipping.Status))
        {
            return Result<ShippingDocumentDto>.Ok(shipping, "Shipping is already confirmed.");
        }

        if (!autoConfirmShipping)
        {
            return Result<ShippingDocumentDto>.Fail(
                "SHIPPING_CONFIRMATION_REQUIRED",
                "Shipping requires confirmation. Confirm shipping or enable AutoConfirmShipping.");
        }

        return await _shippingService
            .ConfirmAsync(new ConfirmShippingRequest
            {
                ShippingId = shipping.Id
            }, userName, cancellationToken)
            .ConfigureAwait(false);
    }

    private static bool IsCompletedStatus(string status)
    {
        return string.Equals(status, "Completed", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "Closed", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "TE", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsConfirmedStatus(string status)
    {
        return string.Equals(status, "Confirmed", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "DeliveryCreated", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "Completed", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "TE", StringComparison.OrdinalIgnoreCase);
    }
}
