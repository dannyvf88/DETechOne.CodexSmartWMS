using DETechOne.SmartWMS.Application.Common;
using DETechOne.SmartWMS.Application.Packing;
using DETechOne.SmartWMS.Application.Picking;
using DETechOne.SmartWMS.Application.SAP;
using DETechOne.SmartWMS.Application.Shipping;
using DETechOne.SmartWMS.Contracts.Dtos.EndToEnd;
using DETechOne.SmartWMS.Contracts.Dtos.Packing;
using DETechOne.SmartWMS.Contracts.Dtos.Picking;
using DETechOne.SmartWMS.Contracts.Dtos.SAP;
using DETechOne.SmartWMS.Contracts.Dtos.Shipping;
using DETechOne.SmartWMS.Contracts.Requests.EndToEnd;
using DETechOne.SmartWMS.Contracts.Requests.Packing;
using DETechOne.SmartWMS.Contracts.Requests.Picking;
using DETechOne.SmartWMS.Contracts.Requests.Shipping;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.EndToEnd;

public sealed class OrderToDeliveryFlowOrchestrator : IEndToEndFlowOrchestrator
{
    private const string SourceDocumentTypeSalesOrder = "SO";
    private const string DefaultPackageCode = "PKG-DEFAULT";

    private readonly ISapSalesOrderReader _salesOrderReader;
    private readonly IPickingService _pickingService;
    private readonly IPackingService _packingService;
    private readonly IShippingService _shippingService;
    private readonly IEndToEndFlowStateStore _stateStore;
    private readonly IClock _clock;

    public OrderToDeliveryFlowOrchestrator(
        ISapSalesOrderReader salesOrderReader,
        IPickingService pickingService,
        IPackingService packingService,
        IShippingService shippingService,
        IEndToEndFlowStateStore stateStore,
        IClock clock)
    {
        _salesOrderReader = salesOrderReader;
        _pickingService = pickingService;
        _packingService = packingService;
        _shippingService = shippingService;
        _stateStore = stateStore;
        _clock = clock;
    }

    public async Task<Result<OrderToDeliveryFlowDto>> StartOrderToDeliveryFlowAsync(
        StartOrderToDeliveryFlowRequest request,
        string userName,
        CancellationToken cancellationToken = default)
    {
        if (request.SalesOrderDocEntry <= 0)
        {
            return Result<OrderToDeliveryFlowDto>.Fail("E2E_VALIDATION", "SalesOrderDocEntry must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(userName))
        {
            return Result<OrderToDeliveryFlowDto>.Fail("E2E_VALIDATION", "UserName is required.");
        }

        Result<SapSalesOrderDto> salesOrderResult = await _salesOrderReader
            .GetByDocEntryAsync(request.SalesOrderDocEntry, cancellationToken)
            .ConfigureAwait(false);

        if (!salesOrderResult.Success || salesOrderResult.Value is null)
        {
            return Result<OrderToDeliveryFlowDto>.Fail(
                salesOrderResult.ErrorCode ?? "SAP_SALES_ORDER_NOT_FOUND",
                salesOrderResult.Message ?? "Sales order could not be read from SAP.");
        }

        SapSalesOrderDto salesOrder = salesOrderResult.Value;
        IReadOnlyList<SapSalesOrderLineDto> openLines = GetOpenLines(salesOrder, request.WarehouseCode);

        if (openLines.Count == 0)
        {
            return Result<OrderToDeliveryFlowDto>.Fail(
                "E2E_NO_OPEN_LINES",
                "Sales order does not have open lines available for the requested warehouse.");
        }

        string warehouseCode = ResolveWarehouseCode(request.WarehouseCode, openLines);
        string correlationId = string.IsNullOrWhiteSpace(request.CorrelationId)
            ? Guid.NewGuid().ToString("N")
            : request.CorrelationId.Trim();

        var state = new OrderToDeliveryFlowState
        {
            FlowId = Guid.NewGuid(),
            CorrelationId = correlationId,
            Status = EndToEndFlowStatus.Created,
            SalesOrderDocEntry = salesOrder.DocEntry,
            SalesOrderDocNum = salesOrder.DocNum,
            CustomerCode = salesOrder.CardCode,
            CustomerName = salesOrder.CardName,
            WarehouseCode = warehouseCode,
            CreatedAtUtc = _clock.UtcNow,
            CreatedBy = userName
        };

        UpsertStep(state, EndToEndFlowStepCode.SapOrderRead, "SAP Sales Order Read", EndToEndFlowStepStatus.Completed, $"Sales order {salesOrder.DocNum} was loaded from SAP.");

        var createPickingRequest = new CreatePickingRequest
        {
            SourceDocumentType = SourceDocumentTypeSalesOrder,
            SourceDocumentNumber = salesOrder.DocNum.ToString(System.Globalization.CultureInfo.InvariantCulture),
            WarehouseCode = warehouseCode,
            Lines = openLines
                .OrderBy(line => line.LineNum)
                .Select(line => new CreatePickingLineRequest
                {
                    LineNumber = line.LineNum,
                    ItemCode = line.ItemCode,
                    WarehouseCode = string.IsNullOrWhiteSpace(line.WarehouseCode) ? warehouseCode : line.WarehouseCode,
                    RequiredQuantity = line.OpenQuantity,
                    UomCode = line.UomCode
                })
                .ToArray()
        };

        Result<PickingDocumentDto> pickingResult = await _pickingService
            .CreateAsync(createPickingRequest, userName, cancellationToken)
            .ConfigureAwait(false);

        if (!pickingResult.Success || pickingResult.Value is null)
        {
            await FailAndSaveAsync(state, EndToEndFlowStepCode.PickingCreate, "Picking Create", pickingResult.Message ?? "Picking document could not be created.", cancellationToken).ConfigureAwait(false);

            return Result<OrderToDeliveryFlowDto>.Fail(
                pickingResult.ErrorCode ?? "E2E_PICKING_CREATE_ERROR",
                pickingResult.Message ?? "Picking document could not be created.");
        }

        state.PickingId = pickingResult.Value.Id;
        state.PickingNumber = pickingResult.Value.PickingNumber;
        state.Status = EndToEndFlowStatus.PickingCreated;
        UpsertStep(state, EndToEndFlowStepCode.PickingCreate, "Picking Create", EndToEndFlowStepStatus.Completed, $"Picking {pickingResult.Value.PickingNumber} was created.");
        UpsertStep(state, EndToEndFlowStepCode.PickingExecution, "Picking Execution", EndToEndFlowStepStatus.Pending, "Waiting for warehouse operator scans.");
        UpsertStep(state, EndToEndFlowStepCode.PickingClose, "Picking Close", EndToEndFlowStepStatus.Pending, "Picking will be closed after all quantities are picked.");
        UpsertStep(state, EndToEndFlowStepCode.PackingCreate, "Packing Create", EndToEndFlowStepStatus.Pending, "Packing will be created after picking is completed.");
        UpsertStep(state, EndToEndFlowStepCode.PackingExecution, "Packing Execution", EndToEndFlowStepStatus.Pending, "Waiting for packing operator confirmation.");
        UpsertStep(state, EndToEndFlowStepCode.PackingClose, "Packing Close", EndToEndFlowStepStatus.Pending, "Packing will be closed after all picked quantities are packed.");
        UpsertStep(state, EndToEndFlowStepCode.ShippingCreate, "Shipping Create", EndToEndFlowStepStatus.Pending, "Shipping will be created after packing is completed.");
        UpsertStep(state, EndToEndFlowStepCode.ShippingConfirm, "Shipping Confirm", EndToEndFlowStepStatus.Pending, "Shipping will be confirmed before SAP delivery creation.");
        UpsertStep(state, EndToEndFlowStepCode.SapDeliveryCreate, "SAP Delivery Create", EndToEndFlowStepStatus.Pending, "Delivery will be created in SAP after shipping confirmation.");

        await _stateStore.SaveAsync(state, cancellationToken).ConfigureAwait(false);

        return Result<OrderToDeliveryFlowDto>.Ok(
            OrderToDeliveryFlowMapper.ToDto(state),
            "End-to-end order to delivery flow started successfully.");
    }

    public async Task<Result<OrderToDeliveryFlowDto>> ExecuteAsync(
        ExecuteOrderToDeliveryFlowRequest request,
        string userName,
        CancellationToken cancellationToken = default)
    {
        if (request.FlowId == Guid.Empty)
        {
            return Result<OrderToDeliveryFlowDto>.Fail("E2E_VALIDATION", "FlowId is required.");
        }

        if (string.IsNullOrWhiteSpace(userName))
        {
            return Result<OrderToDeliveryFlowDto>.Fail("E2E_VALIDATION", "UserName is required.");
        }

        OrderToDeliveryFlowState? state = await _stateStore.GetByIdAsync(request.FlowId, cancellationToken).ConfigureAwait(false);
        if (state is null)
        {
            return Result<OrderToDeliveryFlowDto>.Fail("E2E_NOT_FOUND", "End-to-end flow was not found.");
        }

        if (state.Status == EndToEndFlowStatus.Failed || state.Status == EndToEndFlowStatus.Completed || state.Status == EndToEndFlowStatus.DeliveryCreated)
        {
            return Result<OrderToDeliveryFlowDto>.Ok(OrderToDeliveryFlowMapper.ToDto(state), "Flow does not require execution.");
        }

        Result<OrderToDeliveryFlowDto>? pickingExecutionResult = await ExecutePickingAsync(state, request, userName, cancellationToken).ConfigureAwait(false);
        if (pickingExecutionResult is not null)
        {
            return pickingExecutionResult;
        }

        Result<OrderToDeliveryFlowDto>? packingExecutionResult = await ExecutePackingAsync(state, request, userName, cancellationToken).ConfigureAwait(false);
        if (packingExecutionResult is not null)
        {
            return packingExecutionResult;
        }

        Result<OrderToDeliveryFlowDto>? shippingExecutionResult = await ExecuteShippingAsync(state, request, userName, cancellationToken).ConfigureAwait(false);
        if (shippingExecutionResult is not null)
        {
            return shippingExecutionResult;
        }

        if (request.CreateSapDelivery)
        {
            Result<OrderToDeliveryFlowDto>? deliveryExecutionResult = await ExecuteSapDeliveryAsync(state, userName, cancellationToken).ConfigureAwait(false);
            if (deliveryExecutionResult is not null)
            {
                return deliveryExecutionResult;
            }
        }
        else if (state.ShippingId.HasValue && state.Status == EndToEndFlowStatus.ShippingConfirmed)
        {
            UpsertStep(state, EndToEndFlowStepCode.SapDeliveryCreate, "SAP Delivery Create", EndToEndFlowStepStatus.Skipped, "SAP delivery creation was skipped by request.");
            state.Status = EndToEndFlowStatus.Completed;
        }

        await _stateStore.SaveAsync(state, cancellationToken).ConfigureAwait(false);
        return Result<OrderToDeliveryFlowDto>.Ok(OrderToDeliveryFlowMapper.ToDto(state), "End-to-end flow execution finished.");
    }

    public async Task<Result<OrderToDeliveryFlowDto>> GetByIdAsync(
        Guid flowId,
        CancellationToken cancellationToken = default)
    {
        if (flowId == Guid.Empty)
        {
            return Result<OrderToDeliveryFlowDto>.Fail("E2E_VALIDATION", "FlowId is required.");
        }

        OrderToDeliveryFlowState? state = await _stateStore.GetByIdAsync(flowId, cancellationToken).ConfigureAwait(false);
        if (state is null)
        {
            return Result<OrderToDeliveryFlowDto>.Fail("E2E_NOT_FOUND", "End-to-end flow was not found.");
        }

        return Result<OrderToDeliveryFlowDto>.Ok(OrderToDeliveryFlowMapper.ToDto(state));
    }

    private async Task<Result<OrderToDeliveryFlowDto>?> ExecutePickingAsync(
        OrderToDeliveryFlowState state,
        ExecuteOrderToDeliveryFlowRequest request,
        string userName,
        CancellationToken cancellationToken)
    {
        if (!state.PickingId.HasValue)
        {
            return await FailAndReturnAsync(state, EndToEndFlowStepCode.PickingExecution, "Picking Execution", "Flow does not have a picking document.", cancellationToken).ConfigureAwait(false);
        }

        Result<PickingDocumentDto> pickingResult = await _pickingService.GetByIdAsync(state.PickingId.Value, cancellationToken).ConfigureAwait(false);
        if (!pickingResult.Success || pickingResult.Value is null)
        {
            return await FailAndReturnAsync(state, EndToEndFlowStepCode.PickingExecution, "Picking Execution", pickingResult.Message ?? "Picking document was not found.", cancellationToken).ConfigureAwait(false);
        }

        PickingDocumentDto picking = pickingResult.Value;
        if (picking.PendingQuantity > 0 && !request.AutoCompletePicking)
        {
            state.Status = EndToEndFlowStatus.WaitingForOperator;
            UpsertStep(state, EndToEndFlowStepCode.PickingExecution, "Picking Execution", EndToEndFlowStepStatus.Pending, "Picking has pending quantities. Execute scans or enable AutoCompletePicking.");
            await _stateStore.SaveAsync(state, cancellationToken).ConfigureAwait(false);
            return Result<OrderToDeliveryFlowDto>.Ok(OrderToDeliveryFlowMapper.ToDto(state), "Flow is waiting for picking execution.");
        }

        if (picking.PendingQuantity > 0)
        {
            foreach (PickingLineDto line in picking.Lines.Where(line => line.PendingQuantity > 0).OrderBy(line => line.LineNumber))
            {
                Result<PickingDocumentDto> scanResult = await _pickingService.ScanItemAsync(new ScanPickingItemRequest
                {
                    PickingId = picking.Id,
                    LineNumber = line.LineNumber,
                    ItemCode = line.ItemCode,
                    Quantity = line.PendingQuantity,
                    LotNumber = line.LotNumber
                }, userName, cancellationToken).ConfigureAwait(false);

                if (!scanResult.Success || scanResult.Value is null)
                {
                    return await FailAndReturnAsync(state, EndToEndFlowStepCode.PickingExecution, "Picking Execution", scanResult.Message ?? "Picking scan could not be registered.", cancellationToken).ConfigureAwait(false);
                }

                picking = scanResult.Value;
            }
        }

        UpsertStep(state, EndToEndFlowStepCode.PickingExecution, "Picking Execution", EndToEndFlowStepStatus.Completed, "Picking quantities were completed.");

        if (!IsCompletedStatus(picking.Status))
        {
            Result<PickingDocumentDto> closeResult = await _pickingService.CloseAsync(new ClosePickingRequest
            {
                PickingId = picking.Id
            }, userName, cancellationToken).ConfigureAwait(false);

            if (!closeResult.Success || closeResult.Value is null)
            {
                return await FailAndReturnAsync(state, EndToEndFlowStepCode.PickingClose, "Picking Close", closeResult.Message ?? "Picking document could not be closed.", cancellationToken).ConfigureAwait(false);
            }

            picking = closeResult.Value;
        }

        state.Status = EndToEndFlowStatus.PickingCompleted;
        UpsertStep(state, EndToEndFlowStepCode.PickingClose, "Picking Close", EndToEndFlowStepStatus.Completed, $"Picking {picking.PickingNumber} was closed.");
        await _stateStore.SaveAsync(state, cancellationToken).ConfigureAwait(false);
        return null;
    }

    private async Task<Result<OrderToDeliveryFlowDto>?> ExecutePackingAsync(
        OrderToDeliveryFlowState state,
        ExecuteOrderToDeliveryFlowRequest request,
        string userName,
        CancellationToken cancellationToken)
    {
        if (state.PackingId.HasValue)
        {
            return await ExecuteExistingPackingAsync(state, request, userName, cancellationToken).ConfigureAwait(false);
        }

        if (!state.PickingId.HasValue)
        {
            return await FailAndReturnAsync(state, EndToEndFlowStepCode.PackingCreate, "Packing Create", "Flow does not have a picking document.", cancellationToken).ConfigureAwait(false);
        }

        Result<PickingDocumentDto> pickingResult = await _pickingService.GetByIdAsync(state.PickingId.Value, cancellationToken).ConfigureAwait(false);
        if (!pickingResult.Success || pickingResult.Value is null)
        {
            return await FailAndReturnAsync(state, EndToEndFlowStepCode.PackingCreate, "Packing Create", pickingResult.Message ?? "Picking document was not found.", cancellationToken).ConfigureAwait(false);
        }

        PickingDocumentDto picking = pickingResult.Value;
        if (picking.PickedQuantity <= 0 || picking.PendingQuantity > 0)
        {
            state.Status = EndToEndFlowStatus.WaitingForOperator;
            UpsertStep(state, EndToEndFlowStepCode.PackingCreate, "Packing Create", EndToEndFlowStepStatus.Pending, "Packing cannot be created until picking is completed.");
            await _stateStore.SaveAsync(state, cancellationToken).ConfigureAwait(false);
            return Result<OrderToDeliveryFlowDto>.Ok(OrderToDeliveryFlowMapper.ToDto(state), "Flow is waiting for picking completion.");
        }

        var packingRequest = new CreatePackingRequest
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

        Result<PackingDocumentDto> packingResult = await _packingService.CreateAsync(packingRequest, userName, cancellationToken).ConfigureAwait(false);
        if (!packingResult.Success || packingResult.Value is null)
        {
            return await FailAndReturnAsync(state, EndToEndFlowStepCode.PackingCreate, "Packing Create", packingResult.Message ?? "Packing document could not be created.", cancellationToken).ConfigureAwait(false);
        }

        state.PackingId = packingResult.Value.Id;
        state.PackingNumber = packingResult.Value.PackingNumber;
        state.Status = EndToEndFlowStatus.PackingCreated;
        UpsertStep(state, EndToEndFlowStepCode.PackingCreate, "Packing Create", EndToEndFlowStepStatus.Completed, $"Packing {packingResult.Value.PackingNumber} was created.");
        await _stateStore.SaveAsync(state, cancellationToken).ConfigureAwait(false);

        return await ExecuteExistingPackingAsync(state, request, userName, cancellationToken).ConfigureAwait(false);
    }

    private async Task<Result<OrderToDeliveryFlowDto>?> ExecuteExistingPackingAsync(
        OrderToDeliveryFlowState state,
        ExecuteOrderToDeliveryFlowRequest request,
        string userName,
        CancellationToken cancellationToken)
    {
        if (!state.PackingId.HasValue)
        {
            return await FailAndReturnAsync(state, EndToEndFlowStepCode.PackingExecution, "Packing Execution", "Flow does not have a packing document.", cancellationToken).ConfigureAwait(false);
        }

        Result<PackingDocumentDto> packingResult = await _packingService.GetByIdAsync(state.PackingId.Value, cancellationToken).ConfigureAwait(false);
        if (!packingResult.Success || packingResult.Value is null)
        {
            return await FailAndReturnAsync(state, EndToEndFlowStepCode.PackingExecution, "Packing Execution", packingResult.Message ?? "Packing document was not found.", cancellationToken).ConfigureAwait(false);
        }

        PackingDocumentDto packing = packingResult.Value;
        if (packing.PendingQuantity > 0 && !request.AutoCompletePacking)
        {
            state.Status = EndToEndFlowStatus.WaitingForOperator;
            UpsertStep(state, EndToEndFlowStepCode.PackingExecution, "Packing Execution", EndToEndFlowStepStatus.Pending, "Packing has pending quantities. Pack items or enable AutoCompletePacking.");
            await _stateStore.SaveAsync(state, cancellationToken).ConfigureAwait(false);
            return Result<OrderToDeliveryFlowDto>.Ok(OrderToDeliveryFlowMapper.ToDto(state), "Flow is waiting for packing execution.");
        }

        if (packing.PendingQuantity > 0)
        {
            string packageCode = string.IsNullOrWhiteSpace(request.PackageCode) ? DefaultPackageCode : request.PackageCode.Trim();
            foreach (PackingLineDto line in packing.Lines.Where(line => line.PendingQuantity > 0).OrderBy(line => line.LineNumber))
            {
                Result<PackingDocumentDto> packResult = await _packingService.PackItemAsync(new PackItemRequest
                {
                    PackingId = packing.Id,
                    LineNumber = line.LineNumber,
                    Quantity = line.PendingQuantity,
                    PackageCode = packageCode
                }, userName, cancellationToken).ConfigureAwait(false);

                if (!packResult.Success || packResult.Value is null)
                {
                    return await FailAndReturnAsync(state, EndToEndFlowStepCode.PackingExecution, "Packing Execution", packResult.Message ?? "Packing item could not be registered.", cancellationToken).ConfigureAwait(false);
                }

                packing = packResult.Value;
            }
        }

        UpsertStep(state, EndToEndFlowStepCode.PackingExecution, "Packing Execution", EndToEndFlowStepStatus.Completed, "Packing quantities were completed.");

        if (!IsCompletedStatus(packing.Status.ToString()))
        {
            Result<PackingDocumentDto> closeResult = await _packingService.CloseAsync(new ClosePackingRequest
            {
                PackingId = packing.Id
            }, userName, cancellationToken).ConfigureAwait(false);

            if (!closeResult.Success || closeResult.Value is null)
            {
                return await FailAndReturnAsync(state, EndToEndFlowStepCode.PackingClose, "Packing Close", closeResult.Message ?? "Packing document could not be closed.", cancellationToken).ConfigureAwait(false);
            }

            packing = closeResult.Value;
        }

        state.Status = EndToEndFlowStatus.PackingCompleted;
        UpsertStep(state, EndToEndFlowStepCode.PackingClose, "Packing Close", EndToEndFlowStepStatus.Completed, $"Packing {packing.PackingNumber} was closed.");
        await _stateStore.SaveAsync(state, cancellationToken).ConfigureAwait(false);
        return null;
    }

    private async Task<Result<OrderToDeliveryFlowDto>?> ExecuteShippingAsync(
        OrderToDeliveryFlowState state,
        ExecuteOrderToDeliveryFlowRequest request,
        string userName,
        CancellationToken cancellationToken)
    {
        if (state.ShippingId.HasValue)
        {
            return await ExecuteExistingShippingAsync(state, request, userName, cancellationToken).ConfigureAwait(false);
        }

        if (!state.PackingId.HasValue)
        {
            return await FailAndReturnAsync(state, EndToEndFlowStepCode.ShippingCreate, "Shipping Create", "Flow does not have a packing document.", cancellationToken).ConfigureAwait(false);
        }

        Result<PackingDocumentDto> packingResult = await _packingService.GetByIdAsync(state.PackingId.Value, cancellationToken).ConfigureAwait(false);
        if (!packingResult.Success || packingResult.Value is null)
        {
            return await FailAndReturnAsync(state, EndToEndFlowStepCode.ShippingCreate, "Shipping Create", packingResult.Message ?? "Packing document was not found.", cancellationToken).ConfigureAwait(false);
        }

        PackingDocumentDto packing = packingResult.Value;
        if (packing.PackedQuantity <= 0 || packing.PendingQuantity > 0)
        {
            state.Status = EndToEndFlowStatus.WaitingForOperator;
            UpsertStep(state, EndToEndFlowStepCode.ShippingCreate, "Shipping Create", EndToEndFlowStepStatus.Pending, "Shipping cannot be created until packing is completed.");
            await _stateStore.SaveAsync(state, cancellationToken).ConfigureAwait(false);
            return Result<OrderToDeliveryFlowDto>.Ok(OrderToDeliveryFlowMapper.ToDto(state), "Flow is waiting for packing completion.");
        }

        var shippingRequest = new CreateShippingRequest
        {
            PackingId = packing.Id,
            PackingNumber = packing.PackingNumber,
            WarehouseCode = packing.WarehouseCode,
            CustomerCode = state.CustomerCode,
            CustomerName = state.CustomerName,
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

        Result<ShippingDocumentDto> shippingResult = await _shippingService.CreateAsync(shippingRequest, userName, cancellationToken).ConfigureAwait(false);
        if (!shippingResult.Success || shippingResult.Value is null)
        {
            return await FailAndReturnAsync(state, EndToEndFlowStepCode.ShippingCreate, "Shipping Create", shippingResult.Message ?? "Shipping document could not be created.", cancellationToken).ConfigureAwait(false);
        }

        state.ShippingId = shippingResult.Value.Id;
        state.ShippingNumber = shippingResult.Value.ShippingNumber;
        state.Status = EndToEndFlowStatus.ShippingCreated;
        UpsertStep(state, EndToEndFlowStepCode.ShippingCreate, "Shipping Create", EndToEndFlowStepStatus.Completed, $"Shipping {shippingResult.Value.ShippingNumber} was created.");
        await _stateStore.SaveAsync(state, cancellationToken).ConfigureAwait(false);

        return await ExecuteExistingShippingAsync(state, request, userName, cancellationToken).ConfigureAwait(false);
    }

    private async Task<Result<OrderToDeliveryFlowDto>?> ExecuteExistingShippingAsync(
        OrderToDeliveryFlowState state,
        ExecuteOrderToDeliveryFlowRequest request,
        string userName,
        CancellationToken cancellationToken)
    {
        if (!state.ShippingId.HasValue)
        {
            return await FailAndReturnAsync(state, EndToEndFlowStepCode.ShippingConfirm, "Shipping Confirm", "Flow does not have a shipping document.", cancellationToken).ConfigureAwait(false);
        }

        Result<ShippingDocumentDto> shippingResult = await _shippingService.GetByIdAsync(state.ShippingId.Value, cancellationToken).ConfigureAwait(false);
        if (!shippingResult.Success || shippingResult.Value is null)
        {
            return await FailAndReturnAsync(state, EndToEndFlowStepCode.ShippingConfirm, "Shipping Confirm", shippingResult.Message ?? "Shipping document was not found.", cancellationToken).ConfigureAwait(false);
        }

        ShippingDocumentDto shipping = shippingResult.Value;
        if (!IsConfirmedStatus(shipping.Status) && !request.AutoConfirmShipping)
        {
            state.Status = EndToEndFlowStatus.WaitingForOperator;
            UpsertStep(state, EndToEndFlowStepCode.ShippingConfirm, "Shipping Confirm", EndToEndFlowStepStatus.Pending, "Shipping requires confirmation. Confirm shipping or enable AutoConfirmShipping.");
            await _stateStore.SaveAsync(state, cancellationToken).ConfigureAwait(false);
            return Result<OrderToDeliveryFlowDto>.Ok(OrderToDeliveryFlowMapper.ToDto(state), "Flow is waiting for shipping confirmation.");
        }

        if (!IsConfirmedStatus(shipping.Status))
        {
            Result<ShippingDocumentDto> confirmResult = await _shippingService.ConfirmAsync(new ConfirmShippingRequest
            {
                ShippingId = shipping.Id
            }, userName, cancellationToken).ConfigureAwait(false);

            if (!confirmResult.Success || confirmResult.Value is null)
            {
                return await FailAndReturnAsync(state, EndToEndFlowStepCode.ShippingConfirm, "Shipping Confirm", confirmResult.Message ?? "Shipping document could not be confirmed.", cancellationToken).ConfigureAwait(false);
            }

            shipping = confirmResult.Value;
        }

        state.Status = EndToEndFlowStatus.ShippingConfirmed;
        UpsertStep(state, EndToEndFlowStepCode.ShippingConfirm, "Shipping Confirm", EndToEndFlowStepStatus.Completed, $"Shipping {shipping.ShippingNumber} was confirmed.");
        await _stateStore.SaveAsync(state, cancellationToken).ConfigureAwait(false);
        return null;
    }

    private async Task<Result<OrderToDeliveryFlowDto>?> ExecuteSapDeliveryAsync(
        OrderToDeliveryFlowState state,
        string userName,
        CancellationToken cancellationToken)
    {
        if (!state.ShippingId.HasValue)
        {
            return await FailAndReturnAsync(state, EndToEndFlowStepCode.SapDeliveryCreate, "SAP Delivery Create", "Flow does not have a shipping document.", cancellationToken).ConfigureAwait(false);
        }

        Result<ShippingDocumentDto> deliveryResult = await _shippingService.CreateSapDeliveryAsync(new CreateSapDeliveryRequest
        {
            ShippingId = state.ShippingId.Value
        }, userName, cancellationToken).ConfigureAwait(false);

        if (!deliveryResult.Success || deliveryResult.Value is null)
        {
            return await FailAndReturnAsync(state, EndToEndFlowStepCode.SapDeliveryCreate, "SAP Delivery Create", deliveryResult.Message ?? "SAP delivery could not be created.", cancellationToken).ConfigureAwait(false);
        }

        state.DeliveryDocEntry = deliveryResult.Value.DeliveryDocEntry;
        state.DeliveryDocNum = deliveryResult.Value.DeliveryDocNum;
        state.Status = EndToEndFlowStatus.DeliveryCreated;
        UpsertStep(state, EndToEndFlowStepCode.SapDeliveryCreate, "SAP Delivery Create", EndToEndFlowStepStatus.Completed, $"SAP Delivery {deliveryResult.Value.DeliveryDocNum} was created.");
        await _stateStore.SaveAsync(state, cancellationToken).ConfigureAwait(false);
        return null;
    }

    private async Task<Result<OrderToDeliveryFlowDto>> FailAndReturnAsync(
        OrderToDeliveryFlowState state,
        string stepCode,
        string stepName,
        string message,
        CancellationToken cancellationToken)
    {
        await FailAndSaveAsync(state, stepCode, stepName, message, cancellationToken).ConfigureAwait(false);
        return Result<OrderToDeliveryFlowDto>.Fail("E2E_EXECUTION_ERROR", message);
    }

    private async Task FailAndSaveAsync(
        OrderToDeliveryFlowState state,
        string stepCode,
        string stepName,
        string message,
        CancellationToken cancellationToken)
    {
        state.Status = EndToEndFlowStatus.Failed;
        UpsertStep(state, stepCode, stepName, EndToEndFlowStepStatus.Failed, message);
        await _stateStore.SaveAsync(state, cancellationToken).ConfigureAwait(false);
    }

    private static IReadOnlyList<SapSalesOrderLineDto> GetOpenLines(SapSalesOrderDto salesOrder, string? warehouseCode)
    {
        IEnumerable<SapSalesOrderLineDto> query = salesOrder.Lines
            .Where(line => line.OpenQuantity > 0);

        if (!string.IsNullOrWhiteSpace(warehouseCode))
        {
            query = query.Where(line => string.Equals(
                line.WarehouseCode,
                warehouseCode,
                StringComparison.OrdinalIgnoreCase));
        }

        return query.ToArray();
    }

    private static string ResolveWarehouseCode(string? requestedWarehouseCode, IReadOnlyList<SapSalesOrderLineDto> openLines)
    {
        if (!string.IsNullOrWhiteSpace(requestedWarehouseCode))
        {
            return requestedWarehouseCode.Trim();
        }

        return openLines[0].WarehouseCode;
    }

    private void UpsertStep(OrderToDeliveryFlowState state, string code, string name, string status, string? message)
    {
        EndToEndFlowStepDto? existing = state.Steps.FirstOrDefault(step => step.Code == code);
        var updated = new EndToEndFlowStepDto
        {
            Code = code,
            Name = name,
            Status = status,
            Message = message,
            CompletedAtUtc = status is EndToEndFlowStepStatus.Completed or EndToEndFlowStepStatus.Failed or EndToEndFlowStepStatus.Skipped
                ? _clock.UtcNow
                : null
        };

        if (existing is not null)
        {
            state.Steps.Remove(existing);
        }

        state.Steps.Add(updated);
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
