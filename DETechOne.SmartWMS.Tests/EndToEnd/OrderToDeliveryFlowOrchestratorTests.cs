using DETechOne.SmartWMS.Application.Common;
using DETechOne.SmartWMS.Application.EndToEnd;
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
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Tests.EndToEnd;

public sealed class OrderToDeliveryFlowOrchestratorTests
{
    [Fact]
    public async Task StartOrderToDeliveryFlowAsync_WhenSalesOrderHasOpenLines_CreatesPickingAndInitialFlow()
    {
        var fixture = new EndToEndFixture();

        Result<OrderToDeliveryFlowDto> result = await fixture.Orchestrator.StartOrderToDeliveryFlowAsync(new StartOrderToDeliveryFlowRequest
        {
            SalesOrderDocEntry = 100,
            WarehouseCode = "01",
            CorrelationId = "QA-001"
        }, "qa-user");

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(EndToEndFlowStatus.PickingCreated, result.Value.Status);
        Assert.Equal(100, result.Value.SalesOrderDocEntry);
        Assert.Equal("QA-001", result.Value.CorrelationId);
        Assert.NotNull(result.Value.PickingId);
        Assert.Contains(result.Value.Steps, step => step.Code == EndToEndFlowStepCode.SapOrderRead && step.Status == EndToEndFlowStepStatus.Completed);
        Assert.Contains(result.Value.Steps, step => step.Code == EndToEndFlowStepCode.PickingCreate && step.Status == EndToEndFlowStepStatus.Completed);
        Assert.Single(fixture.Picking.CreatedRequests);
    }

    [Fact]
    public async Task StartOrderToDeliveryFlowAsync_WhenSalesOrderHasNoOpenLines_ReturnsValidationFailure()
    {
        var fixture = new EndToEndFixture();
        fixture.SalesOrder.SalesOrder = new SapSalesOrderDto
        {
            DocEntry = 101,
            DocNum = 5002,
            CardCode = "C0002",
            CardName = "Cliente sin pendientes",
            DocStatus = "O",
            Lines = new[]
            {
                new SapSalesOrderLineDto
                {
                    LineNum = 0,
                    ItemCode = "A0002",
                    WarehouseCode = "01",
                    Quantity = 5,
                    OpenQuantity = 0,
                    UomCode = "PZA"
                }
            }
        };

        Result<OrderToDeliveryFlowDto> result = await fixture.Orchestrator.StartOrderToDeliveryFlowAsync(new StartOrderToDeliveryFlowRequest
        {
            SalesOrderDocEntry = 101,
            WarehouseCode = "01"
        }, "qa-user");

        Assert.False(result.Success);
        Assert.Equal("E2E_NO_OPEN_LINES", result.ErrorCode);
        Assert.Empty(fixture.Picking.CreatedRequests);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAllAutomationFlagsEnabled_CompletesDeliveryCreation()
    {
        var fixture = new EndToEndFixture();

        Result<OrderToDeliveryFlowDto> startResult = await fixture.Orchestrator.StartOrderToDeliveryFlowAsync(new StartOrderToDeliveryFlowRequest
        {
            SalesOrderDocEntry = 100,
            WarehouseCode = "01"
        }, "qa-user");

        Assert.True(startResult.Success);
        Assert.NotNull(startResult.Value);

        Result<OrderToDeliveryFlowDto> executionResult = await fixture.Orchestrator.ExecuteAsync(new ExecuteOrderToDeliveryFlowRequest
        {
            FlowId = startResult.Value.FlowId,
            AutoCompletePicking = true,
            AutoCompletePacking = true,
            AutoConfirmShipping = true,
            CreateSapDelivery = true,
            PackageCode = "PKG-QA"
        }, "qa-user");

        Assert.True(executionResult.Success);
        Assert.NotNull(executionResult.Value);
        Assert.Equal(EndToEndFlowStatus.DeliveryCreated, executionResult.Value.Status);
        Assert.Equal(9001, executionResult.Value.DeliveryDocEntry);
        Assert.Equal(70001, executionResult.Value.DeliveryDocNum);
        Assert.Contains(executionResult.Value.Steps, step => step.Code == EndToEndFlowStepCode.PickingExecution && step.Status == EndToEndFlowStepStatus.Completed);
        Assert.Contains(executionResult.Value.Steps, step => step.Code == EndToEndFlowStepCode.PackingExecution && step.Status == EndToEndFlowStepStatus.Completed);
        Assert.Contains(executionResult.Value.Steps, step => step.Code == EndToEndFlowStepCode.ShippingConfirm && step.Status == EndToEndFlowStepStatus.Completed);
        Assert.Contains(executionResult.Value.Steps, step => step.Code == EndToEndFlowStepCode.SapDeliveryCreate && step.Status == EndToEndFlowStepStatus.Completed);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPickingIsPendingAndAutoCompleteIsDisabled_WaitsForOperator()
    {
        var fixture = new EndToEndFixture();

        Result<OrderToDeliveryFlowDto> startResult = await fixture.Orchestrator.StartOrderToDeliveryFlowAsync(new StartOrderToDeliveryFlowRequest
        {
            SalesOrderDocEntry = 100,
            WarehouseCode = "01"
        }, "qa-user");

        Assert.True(startResult.Success);
        Assert.NotNull(startResult.Value);

        Result<OrderToDeliveryFlowDto> executionResult = await fixture.Orchestrator.ExecuteAsync(new ExecuteOrderToDeliveryFlowRequest
        {
            FlowId = startResult.Value.FlowId,
            AutoCompletePicking = false
        }, "qa-user");

        Assert.True(executionResult.Success);
        Assert.NotNull(executionResult.Value);
        Assert.Equal(EndToEndFlowStatus.WaitingForOperator, executionResult.Value.Status);
        Assert.Contains(executionResult.Value.Steps, step => step.Code == EndToEndFlowStepCode.PickingExecution && step.Status == EndToEndFlowStepStatus.Pending);
    }

    private sealed class EndToEndFixture
    {
        public EndToEndFixture()
        {
            SalesOrder = new FakeSapSalesOrderReader();
            Picking = new FakePickingService();
            Packing = new FakePackingService();
            Shipping = new FakeShippingService();
            StateStore = new InMemoryEndToEndFlowStateStore();
            Clock = new FixedClock(new DateTime(2026, 06, 02, 12, 0, 0, DateTimeKind.Utc));

            Orchestrator = new OrderToDeliveryFlowOrchestrator(
                SalesOrder,
                Picking,
                Packing,
                Shipping,
                StateStore,
                Clock);
        }

        public FakeSapSalesOrderReader SalesOrder { get; }
        public FakePickingService Picking { get; }
        public FakePackingService Packing { get; }
        public FakeShippingService Shipping { get; }
        public InMemoryEndToEndFlowStateStore StateStore { get; }
        public FixedClock Clock { get; }
        public OrderToDeliveryFlowOrchestrator Orchestrator { get; }
    }

    private sealed class FixedClock : IClock
    {
        public FixedClock(DateTime utcNow) => UtcNow = utcNow;
        public DateTime UtcNow { get; }
    }

    private sealed class FakeSapSalesOrderReader : ISapSalesOrderReader
    {
        public SapSalesOrderDto SalesOrder { get; set; } = new()
        {
            DocEntry = 100,
            DocNum = 5001,
            CardCode = "C0001",
            CardName = "Cliente QA",
            DocStatus = "O",
            DocDate = new DateTime(2026, 06, 02),
            Lines = new[]
            {
                new SapSalesOrderLineDto
                {
                    LineNum = 0,
                    ItemCode = "A0001",
                    ItemDescription = "Articulo QA",
                    WarehouseCode = "01",
                    Quantity = 10,
                    OpenQuantity = 10,
                    UomCode = "PZA"
                }
            }
        };

        public Task<Result<SapSalesOrderDto>> GetByDocEntryAsync(int docEntry, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<SapSalesOrderDto>.Ok(SalesOrder));
    }

    private sealed class FakePickingService : IPickingService
    {
        private readonly Dictionary<Guid, PickingDocumentDto> _documents = new();
        public List<CreatePickingRequest> CreatedRequests { get; } = new();

        public Task<Result<IReadOnlyList<PickingDocumentDto>>> GetOpenAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(Result<IReadOnlyList<PickingDocumentDto>>.Ok(_documents.Values.Where(document => document.Status != "Completed").ToArray()));

        public Task<Result<PickingDocumentDto>> GetByIdAsync(Guid pickingId, CancellationToken cancellationToken = default)
            => Task.FromResult(_documents.TryGetValue(pickingId, out PickingDocumentDto? document)
                ? Result<PickingDocumentDto>.Ok(document)
                : Result<PickingDocumentDto>.Fail("PICKING_NOT_FOUND", "Picking not found."));

        public Task<Result<PickingDocumentDto>> CreateAsync(CreatePickingRequest request, string? userName, CancellationToken cancellationToken = default)
        {
            CreatedRequests.Add(request);
            Guid id = Guid.NewGuid();
            PickingDocumentDto document = new()
            {
                Id = id,
                PickingNumber = "PK-0001",
                SourceDocumentType = request.SourceDocumentType,
                SourceDocumentNumber = request.SourceDocumentNumber,
                WarehouseCode = request.WarehouseCode,
                RequestedBy = userName ?? string.Empty,
                Status = "Open",
                RequiredQuantity = request.Lines.Sum(line => line.RequiredQuantity),
                PickedQuantity = 0,
                PendingQuantity = request.Lines.Sum(line => line.RequiredQuantity),
                Lines = request.Lines.Select(line => new PickingLineDto
                {
                    Id = Guid.NewGuid(),
                    LineNumber = line.LineNumber,
                    ItemCode = line.ItemCode,
                    WarehouseCode = line.WarehouseCode,
                    LocationCode = line.LocationCode,
                    RequiredQuantity = line.RequiredQuantity,
                    PickedQuantity = 0,
                    PendingQuantity = line.RequiredQuantity,
                    LotNumber = line.LotNumber,
                    UomCode = line.UomCode,
                    Status = "Pending"
                }).ToArray()
            };

            _documents[id] = document;
            return Task.FromResult(Result<PickingDocumentDto>.Ok(document));
        }

        public Task<Result<PickingDocumentDto>> ScanItemAsync(ScanPickingItemRequest request, string? userName, CancellationToken cancellationToken = default)
        {
            if (!_documents.TryGetValue(request.PickingId, out PickingDocumentDto? document))
            {
                return Task.FromResult(Result<PickingDocumentDto>.Fail("PICKING_NOT_FOUND", "Picking not found."));
            }

            PickingLineDto[] lines = document.Lines.Select(line => line.LineNumber == request.LineNumber
                ? new PickingLineDto
                {
                    Id = line.Id,
                    LineNumber = line.LineNumber,
                    ItemCode = line.ItemCode,
                    WarehouseCode = line.WarehouseCode,
                    LocationCode = line.LocationCode,
                    RequiredQuantity = line.RequiredQuantity,
                    PickedQuantity = line.PickedQuantity + request.Quantity,
                    PendingQuantity = Math.Max(0, line.PendingQuantity - request.Quantity),
                    LotNumber = line.LotNumber,
                    UomCode = line.UomCode,
                    Status = line.PendingQuantity - request.Quantity <= 0 ? "Completed" : "Partial"
                }
                : line).ToArray();

            document = CopyPicking(document, lines, "InProgress");
            _documents[request.PickingId] = document;
            return Task.FromResult(Result<PickingDocumentDto>.Ok(document));
        }

        public Task<Result<PickingDocumentDto>> CloseAsync(ClosePickingRequest request, string? userName, CancellationToken cancellationToken = default)
        {
            if (!_documents.TryGetValue(request.PickingId, out PickingDocumentDto? document))
            {
                return Task.FromResult(Result<PickingDocumentDto>.Fail("PICKING_NOT_FOUND", "Picking not found."));
            }

            document = CopyPicking(document, document.Lines, "Completed");
            document.CompletedAtUtc = DateTime.UtcNow;
            document.CompletedBy = userName;
            _documents[request.PickingId] = document;
            return Task.FromResult(Result<PickingDocumentDto>.Ok(document));
        }

        public Task<Result<PickingDocumentDto>> CancelAsync(CancelPickingRequest request, string? userName, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<PickingDocumentDto>.Fail("NOT_IMPLEMENTED", "Not needed for QA tests."));

        private static PickingDocumentDto CopyPicking(PickingDocumentDto source, IReadOnlyList<PickingLineDto> lines, string status)
            => new()
            {
                Id = source.Id,
                PickingNumber = source.PickingNumber,
                SourceDocumentType = source.SourceDocumentType,
                SourceDocumentNumber = source.SourceDocumentNumber,
                WarehouseCode = source.WarehouseCode,
                RequestedBy = source.RequestedBy,
                Status = status,
                RequiredQuantity = lines.Sum(line => line.RequiredQuantity),
                PickedQuantity = lines.Sum(line => line.PickedQuantity),
                PendingQuantity = lines.Sum(line => line.PendingQuantity),
                StartedAtUtc = source.StartedAtUtc,
                StartedBy = source.StartedBy,
                CompletedAtUtc = source.CompletedAtUtc,
                CompletedBy = source.CompletedBy,
                CancelledAtUtc = source.CancelledAtUtc,
                CancelledBy = source.CancelledBy,
                CancelReason = source.CancelReason,
                Lines = lines
            };
    }

    private sealed class FakePackingService : IPackingService
    {
        private readonly Dictionary<Guid, PackingDocumentDto> _documents = new();

        public Task<Result<IReadOnlyList<PackingDocumentDto>>> GetOpenAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(Result<IReadOnlyList<PackingDocumentDto>>.Ok(_documents.Values.Where(document => document.Status != PackingStatus.Completed).ToArray()));

        public Task<Result<PackingDocumentDto>> GetByIdAsync(Guid packingId, CancellationToken cancellationToken = default)
            => Task.FromResult(_documents.TryGetValue(packingId, out PackingDocumentDto? document)
                ? Result<PackingDocumentDto>.Ok(document)
                : Result<PackingDocumentDto>.Fail("PACKING_NOT_FOUND", "Packing not found."));

        public Task<Result<PackingDocumentDto>> CreateAsync(CreatePackingRequest request, string? userName, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.NewGuid();
            PackingLineDto[] lines = request.Lines.Select(line => new PackingLineDto(
                Guid.NewGuid(),
                line.LineNumber,
                line.ItemCode,
                line.WarehouseCode,
                line.LocationCode,
                line.PickedQuantity,
                0,
                line.PickedQuantity,
                line.LotNumber,
                line.UomCode,
                null,
                PackingLineStatus.Pending)).ToArray();

            PackingDocumentDto document = new(
                id,
                "PA-0001",
                request.PickingId,
                request.PickingNumber,
                request.WarehouseCode,
                userName ?? string.Empty,
                PackingStatus.Open,
                lines.Sum(line => line.PickedQuantity),
                0,
                lines.Sum(line => line.PendingQuantity),
                DateTime.UtcNow,
                null,
                null,
                null,
                lines);

            _documents[id] = document;
            return Task.FromResult(Result<PackingDocumentDto>.Ok(document));
        }

        public Task<Result<PackingDocumentDto>> PackItemAsync(PackItemRequest request, string? userName, CancellationToken cancellationToken = default)
        {
            if (!_documents.TryGetValue(request.PackingId, out PackingDocumentDto? document))
            {
                return Task.FromResult(Result<PackingDocumentDto>.Fail("PACKING_NOT_FOUND", "Packing not found."));
            }

            PackingLineDto[] lines = document.Lines.Select(line => line.LineNumber == request.LineNumber
                ? line with
                {
                    PackedQuantity = line.PackedQuantity + request.Quantity,
                    PendingQuantity = Math.Max(0, line.PendingQuantity - request.Quantity),
                    PackageCode = request.PackageCode,
                    Status = line.PendingQuantity - request.Quantity <= 0 ? PackingLineStatus.Completed : PackingLineStatus.Partial
                }
                : line).ToArray();

            document = document with
            {
                Status = PackingStatus.InProgress,
                PackedQuantity = lines.Sum(line => line.PackedQuantity),
                PendingQuantity = lines.Sum(line => line.PendingQuantity),
                Lines = lines
            };
            _documents[request.PackingId] = document;
            return Task.FromResult(Result<PackingDocumentDto>.Ok(document));
        }

        public Task<Result<PackingDocumentDto>> CloseAsync(ClosePackingRequest request, string? userName, CancellationToken cancellationToken = default)
        {
            if (!_documents.TryGetValue(request.PackingId, out PackingDocumentDto? document))
            {
                return Task.FromResult(Result<PackingDocumentDto>.Fail("PACKING_NOT_FOUND", "Packing not found."));
            }

            document = document with
            {
                Status = PackingStatus.Completed,
                CompletedAtUtc = DateTime.UtcNow
            };
            _documents[request.PackingId] = document;
            return Task.FromResult(Result<PackingDocumentDto>.Ok(document));
        }

        public Task<Result<PackingDocumentDto>> CancelAsync(CancelPackingRequest request, string? userName, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<PackingDocumentDto>.Fail("NOT_IMPLEMENTED", "Not needed for QA tests."));
    }

    private sealed class FakeShippingService : IShippingService
    {
        private readonly Dictionary<Guid, ShippingDocumentDto> _documents = new();

        public Task<Result<IReadOnlyList<ShippingDocumentDto>>> GetOpenAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(Result<IReadOnlyList<ShippingDocumentDto>>.Ok(_documents.Values.Where(document => document.Status != "Confirmed" && document.Status != "DeliveryCreated").ToArray()));

        public Task<Result<ShippingDocumentDto>> GetByIdAsync(Guid shippingId, CancellationToken cancellationToken = default)
            => Task.FromResult(_documents.TryGetValue(shippingId, out ShippingDocumentDto? document)
                ? Result<ShippingDocumentDto>.Ok(document)
                : Result<ShippingDocumentDto>.Fail("SHIPPING_NOT_FOUND", "Shipping not found."));

        public Task<Result<ShippingDocumentDto>> CreateAsync(CreateShippingRequest request, string? userName, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.NewGuid();
            ShippingDocumentDto document = new()
            {
                Id = id,
                ShippingNumber = "SH-0001",
                PackingId = request.PackingId,
                PackingNumber = request.PackingNumber,
                WarehouseCode = request.WarehouseCode,
                CustomerCode = request.CustomerCode,
                CustomerName = request.CustomerName,
                RequestedBy = userName ?? string.Empty,
                Status = "Open",
                PackedQuantity = request.Lines.Sum(line => line.PackedQuantity),
                CreatedAtUtc = DateTime.UtcNow,
                Lines = request.Lines.Select(line => new ShippingLineDto
                {
                    LineNumber = line.LineNumber,
                    ItemCode = line.ItemCode,
                    WarehouseCode = line.WarehouseCode,
                    LocationCode = line.LocationCode,
                    PackedQuantity = line.PackedQuantity,
                    LotNumber = line.LotNumber,
                    UomCode = line.UomCode,
                    Status = "Open"
                }).ToArray()
            };
            _documents[id] = document;
            return Task.FromResult(Result<ShippingDocumentDto>.Ok(document));
        }

        public Task<Result<ShippingDocumentDto>> ConfirmAsync(ConfirmShippingRequest request, string? userName, CancellationToken cancellationToken = default)
        {
            if (!_documents.TryGetValue(request.ShippingId, out ShippingDocumentDto? document))
            {
                return Task.FromResult(Result<ShippingDocumentDto>.Fail("SHIPPING_NOT_FOUND", "Shipping not found."));
            }

            document = CopyShipping(document, "Confirmed", null, null, userName);
            _documents[request.ShippingId] = document;
            return Task.FromResult(Result<ShippingDocumentDto>.Ok(document));
        }

        public Task<Result<ShippingDocumentDto>> CreateSapDeliveryAsync(CreateSapDeliveryRequest request, string? userName, CancellationToken cancellationToken = default)
        {
            if (!_documents.TryGetValue(request.ShippingId, out ShippingDocumentDto? document))
            {
                return Task.FromResult(Result<ShippingDocumentDto>.Fail("SHIPPING_NOT_FOUND", "Shipping not found."));
            }

            document = CopyShipping(document, "DeliveryCreated", 9001, 70001, userName);
            _documents[request.ShippingId] = document;
            return Task.FromResult(Result<ShippingDocumentDto>.Ok(document));
        }

        public Task<Result<ShippingDocumentDto>> CancelAsync(CancelShippingRequest request, string? userName, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<ShippingDocumentDto>.Fail("NOT_IMPLEMENTED", "Not needed for QA tests."));

        private static ShippingDocumentDto CopyShipping(ShippingDocumentDto source, string status, int? deliveryDocEntry, int? deliveryDocNum, string? userName)
            => new()
            {
                Id = source.Id,
                ShippingNumber = source.ShippingNumber,
                PackingId = source.PackingId,
                PackingNumber = source.PackingNumber,
                WarehouseCode = source.WarehouseCode,
                CustomerCode = source.CustomerCode,
                CustomerName = source.CustomerName,
                RequestedBy = source.RequestedBy,
                Status = status,
                PackedQuantity = source.PackedQuantity,
                CreatedAtUtc = source.CreatedAtUtc,
                ConfirmedAtUtc = status is "Confirmed" or "DeliveryCreated" ? DateTime.UtcNow : source.ConfirmedAtUtc,
                ConfirmedBy = status is "Confirmed" or "DeliveryCreated" ? userName : source.ConfirmedBy,
                DeliveryDocEntry = deliveryDocEntry ?? source.DeliveryDocEntry,
                DeliveryDocNum = deliveryDocNum ?? source.DeliveryDocNum,
                DeliveryCreatedAtUtc = deliveryDocEntry.HasValue ? DateTime.UtcNow : source.DeliveryCreatedAtUtc,
                DeliveryCreatedBy = deliveryDocEntry.HasValue ? userName : source.DeliveryCreatedBy,
                CancelledAtUtc = source.CancelledAtUtc,
                CancelledBy = source.CancelledBy,
                CancelReason = source.CancelReason,
                Lines = source.Lines.Select(line => new ShippingLineDto
                {
                    LineNumber = line.LineNumber,
                    ItemCode = line.ItemCode,
                    WarehouseCode = line.WarehouseCode,
                    LocationCode = line.LocationCode,
                    PackedQuantity = line.PackedQuantity,
                    LotNumber = line.LotNumber,
                    UomCode = line.UomCode,
                    Status = status,
                    ConfirmedAtUtc = status is "Confirmed" or "DeliveryCreated" ? DateTime.UtcNow : line.ConfirmedAtUtc,
                    ConfirmedBy = status is "Confirmed" or "DeliveryCreated" ? userName : line.ConfirmedBy
                }).ToArray()
            };
    }
}
