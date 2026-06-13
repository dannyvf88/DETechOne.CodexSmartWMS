using DETechOne.SmartWMS.Application.Alerts;
using DETechOne.SmartWMS.Application.Common;
using DETechOne.SmartWMS.Application.Packing;
using DETechOne.SmartWMS.Application.Picking;
using DETechOne.SmartWMS.Application.Shipping;
using DETechOne.SmartWMS.Contracts.Dtos.Dashboard;
using DETechOne.SmartWMS.Contracts.Requests.Alerts;
using DETechOne.SmartWMS.Contracts.Requests.Dashboard;
using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Entities.Packing;
using DETechOne.SmartWMS.Domain.Entities.Picking;
using DETechOne.SmartWMS.Domain.Entities.Shipping;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Application.Dashboard;

public sealed class DashboardMetricsService : IDashboardMetricsService
{
    private readonly IPickingRepository _pickingRepository;
    private readonly IPackingRepository _packingRepository;
    private readonly IShippingRepository _shippingRepository;
    private readonly IAlertRepository _alertRepository;
    private readonly IDeviceMetricsReader _deviceMetricsReader;
    private readonly IClock _clock;

    public DashboardMetricsService(
        IPickingRepository pickingRepository,
        IPackingRepository packingRepository,
        IShippingRepository shippingRepository,
        IAlertRepository alertRepository,
        IDeviceMetricsReader deviceMetricsReader,
        IClock clock)
    {
        _pickingRepository = pickingRepository;
        _packingRepository = packingRepository;
        _shippingRepository = shippingRepository;
        _alertRepository = alertRepository;
        _deviceMetricsReader = deviceMetricsReader;
        _clock = clock;
    }

    public async Task<Result<DashboardOverviewDto>> GetOverviewAsync(DashboardMetricsRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.FromUtc.HasValue && request.ToUtc.HasValue && request.FromUtc > request.ToUtc)
        {
            return Result<DashboardOverviewDto>.Fail("INVALID_DATE_RANGE", "FromUtc cannot be greater than ToUtc.");
        }

        IReadOnlyList<PickingDocument> pickings = await _pickingRepository.GetOpenAsync(cancellationToken).ConfigureAwait(false);
        IReadOnlyList<PackingDocument> packings = await _packingRepository.GetOpenAsync(cancellationToken).ConfigureAwait(false);
        IReadOnlyList<ShippingDocument> shippings = await _shippingRepository.GetOpenAsync(cancellationToken).ConfigureAwait(false);
        DeviceMetricsDto deviceMetrics = await _deviceMetricsReader.GetMetricsAsync(cancellationToken).ConfigureAwait(false);
        AlertMetricsDto alertMetrics = await BuildAlertMetricsAsync(request, cancellationToken).ConfigureAwait(false);

        OperationStatusMetricsDto pickingMetrics = BuildPickingMetrics(FilterWarehouse(pickings, request.WarehouseCode));
        OperationStatusMetricsDto packingMetrics = BuildPackingMetrics(FilterWarehouse(packings, request.WarehouseCode));
        OperationStatusMetricsDto shippingMetrics = BuildShippingMetrics(FilterWarehouse(shippings, request.WarehouseCode));

        var counters = new List<MetricCounterDto>
        {
            Counter("PICKING_OPEN", "Picking abiertos", pickingMetrics.Open, "docs"),
            Counter("PACKING_OPEN", "Packing abiertos", packingMetrics.Open, "docs"),
            Counter("SHIPPING_OPEN", "Shipping abiertos", shippingMetrics.Open, "docs"),
            Counter("DEVICES_ONLINE", "Dispositivos en línea", deviceMetrics.Online, "devices"),
            Counter("ALERTS_OPEN", "Alertas abiertas", alertMetrics.Open, "alerts"),
            Counter("ALERTS_CRITICAL", "Alertas críticas", alertMetrics.Critical, "alerts"),
            Counter("PICKING_PENDING_QTY", "Cantidad pendiente de picking", pickingMetrics.PendingQuantity, "qty"),
            Counter("PACKING_PENDING_QTY", "Cantidad pendiente de packing", packingMetrics.PendingQuantity, "qty")
        };

        var overview = new DashboardOverviewDto
        {
            GeneratedAtUtc = _clock.UtcNow,
            FromUtc = request.FromUtc,
            ToUtc = request.ToUtc,
            WarehouseCode = NormalizeOptional(request.WarehouseCode),
            Picking = pickingMetrics,
            Packing = packingMetrics,
            Shipping = shippingMetrics,
            Devices = deviceMetrics,
            Alerts = alertMetrics,
            Counters = counters
        };

        return Result<DashboardOverviewDto>.Ok(overview, "Dashboard metrics generated successfully.");
    }

    private async Task<AlertMetricsDto> BuildAlertMetricsAsync(DashboardMetricsRequest request, CancellationToken cancellationToken)
    {
        var query = new AlertQueryRequest
        {
            FromUtc = request.FromUtc,
            ToUtc = request.ToUtc,
            Page = 1,
            PageSize = 500
        };

        var search = await _alertRepository.SearchAsync(query, cancellationToken).ConfigureAwait(false);
        var alerts = search.Items;

        return new AlertMetricsDto
        {
            Open = alerts.Count(alert => alert.Status == AlertStatus.Open),
            Acknowledged = alerts.Count(alert => alert.Status == AlertStatus.Acknowledged),
            Resolved = alerts.Count(alert => alert.Status == AlertStatus.Resolved),
            Critical = alerts.Count(alert => alert.Severity == AlertSeverity.Critical),
            Error = alerts.Count(alert => alert.Severity == AlertSeverity.Error),
            Warning = alerts.Count(alert => alert.Severity == AlertSeverity.Warning)
        };
    }

    private static OperationStatusMetricsDto BuildPickingMetrics(IEnumerable<PickingDocument> pickings)
    {
        PickingDocument[] items = pickings.ToArray();
        return new OperationStatusMetricsDto
        {
            Open = items.Count(item => item.Status == PickingStatus.Open),
            InProgress = items.Count(item => item.Status == PickingStatus.InProgress),
            Completed = items.Count(item => item.Status == PickingStatus.Completed),
            Cancelled = items.Count(item => item.Status == PickingStatus.Cancelled),
            RequiredQuantity = items.Sum(item => item.RequiredQuantity),
            ProcessedQuantity = items.Sum(item => item.PickedQuantity),
            PendingQuantity = items.Sum(item => item.PendingQuantity)
        };
    }

    private static OperationStatusMetricsDto BuildPackingMetrics(IEnumerable<PackingDocument> packings)
    {
        PackingDocument[] items = packings.ToArray();
        return new OperationStatusMetricsDto
        {
            Open = items.Count(item => item.Status == PackingStatus.Open),
            InProgress = items.Count(item => item.Status == PackingStatus.InProgress),
            Completed = items.Count(item => item.Status == PackingStatus.Completed),
            Cancelled = items.Count(item => item.Status == PackingStatus.Cancelled),
            RequiredQuantity = items.Sum(item => item.PickedQuantity),
            ProcessedQuantity = items.Sum(item => item.PackedQuantity),
            PendingQuantity = items.Sum(item => item.PendingQuantity)
        };
    }

    private static OperationStatusMetricsDto BuildShippingMetrics(IEnumerable<ShippingDocument> shippings)
    {
        ShippingDocument[] items = shippings.ToArray();
        return new OperationStatusMetricsDto
        {
            Open = items.Count(item => item.Status == ShippingStatus.Open),
            InProgress = items.Count(item => item.Status == ShippingStatus.InProgress),
            Completed = items.Count(item => item.Status == ShippingStatus.Confirmed),
            Cancelled = items.Count(item => item.Status == ShippingStatus.Cancelled),
            DeliveryCreated = items.Count(item => item.Status == ShippingStatus.DeliveryCreated),
            RequiredQuantity = items.Sum(item => item.PackedQuantity),
            ProcessedQuantity = items.Where(item => item.Status == ShippingStatus.Confirmed || item.Status == ShippingStatus.DeliveryCreated).Sum(item => item.PackedQuantity),
            PendingQuantity = items.Where(item => item.Status == ShippingStatus.Open || item.Status == ShippingStatus.InProgress).Sum(item => item.PackedQuantity)
        };
    }

    private static IEnumerable<T> FilterWarehouse<T>(IEnumerable<T> documents, string? warehouseCode)
    {
        string? normalizedWarehouse = NormalizeOptional(warehouseCode);
        if (normalizedWarehouse is null)
        {
            return documents;
        }

        return documents.Where(document => GetWarehouseCode(document) == normalizedWarehouse);
    }

    private static string? GetWarehouseCode<T>(T document)
    {
        return document switch
        {
            PickingDocument picking => NormalizeOptional(picking.WarehouseCode),
            PackingDocument packing => NormalizeOptional(packing.WarehouseCode),
            ShippingDocument shipping => NormalizeOptional(shipping.WarehouseCode),
            _ => null
        };
    }

    private static MetricCounterDto Counter(string code, string label, decimal value, string unit)
    {
        return new MetricCounterDto
        {
            Code = code,
            Label = label,
            Value = value,
            Unit = unit
        };
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }
}
