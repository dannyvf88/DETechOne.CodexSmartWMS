using System.Diagnostics;
using DETechOne.SmartWMS.Application.Alerts;
using DETechOne.SmartWMS.Application.Audit;
using DETechOne.SmartWMS.Application.Common;
using DETechOne.SmartWMS.Application.Dashboard;
using DETechOne.SmartWMS.Application.EndToEnd;
using DETechOne.SmartWMS.Application.Inventory;
using DETechOne.SmartWMS.Application.Movement;
using DETechOne.SmartWMS.Application.Packing;
using DETechOne.SmartWMS.Application.Persistence;
using DETechOne.SmartWMS.Application.Picking;
using DETechOne.SmartWMS.Application.SAP;
using DETechOne.SmartWMS.Application.Schema;
using DETechOne.SmartWMS.Application.Shipping;
using DETechOne.SmartWMS.Contracts.Dtos.Stabilization;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Application.Stabilization;

public sealed class MvpReadinessService : IMvpReadinessService
{
    private readonly IClock _clock;
    private readonly IDatabaseHealthCheck _databaseHealthCheck;
    private readonly IInventoryService _inventoryService;
    private readonly IMovementService _movementService;
    private readonly IPickingService _pickingService;
    private readonly IPackingService _packingService;
    private readonly IShippingService _shippingService;
    private readonly IEndToEndFlowOrchestrator _endToEndFlowOrchestrator;
    private readonly ISapConnectionManager _sapConnectionManager;
    private readonly IDatabaseSchemaInstaller _databaseSchemaInstaller;
    private readonly IAuditService _auditService;
    private readonly IAlertService _alertService;
    private readonly IDashboardMetricsService _dashboardMetricsService;

    public MvpReadinessService(
        IClock clock,
        IDatabaseHealthCheck databaseHealthCheck,
        IInventoryService inventoryService,
        IMovementService movementService,
        IPickingService pickingService,
        IPackingService packingService,
        IShippingService shippingService,
        IEndToEndFlowOrchestrator endToEndFlowOrchestrator,
        ISapConnectionManager sapConnectionManager,
        IDatabaseSchemaInstaller databaseSchemaInstaller,
        IAuditService auditService,
        IAlertService alertService,
        IDashboardMetricsService dashboardMetricsService)
    {
        _clock = clock;
        _databaseHealthCheck = databaseHealthCheck;
        _inventoryService = inventoryService;
        _movementService = movementService;
        _pickingService = pickingService;
        _packingService = packingService;
        _shippingService = shippingService;
        _endToEndFlowOrchestrator = endToEndFlowOrchestrator;
        _sapConnectionManager = sapConnectionManager;
        _databaseSchemaInstaller = databaseSchemaInstaller;
        _auditService = auditService;
        _alertService = alertService;
        _dashboardMetricsService = dashboardMetricsService;
    }

    public async Task<Result<MvpReadinessDto>> CheckAsync(CancellationToken cancellationToken = default)
    {
        var checks = new List<MvpReadinessCheckDto>
        {
            CreateDependencyCheck("CORE_CLOCK", "Core clock", _clock),
            CreateDependencyCheck("INVENTORY_ENGINE", "Inventory engine", _inventoryService),
            CreateDependencyCheck("MOVEMENT_ENGINE", "Movement engine", _movementService),
            CreateDependencyCheck("PICKING_ENGINE", "Picking engine", _pickingService),
            CreateDependencyCheck("PACKING_ENGINE", "Packing engine", _packingService),
            CreateDependencyCheck("SHIPPING_ENGINE", "Shipping engine", _shippingService),
            CreateDependencyCheck("END_TO_END_ORCHESTRATOR", "End-to-End orchestrator", _endToEndFlowOrchestrator),
            CreateDependencyCheck("SAP_CONNECTION_MANAGER", "SAP connection manager", _sapConnectionManager),
            CreateDependencyCheck("SCHEMA_INSTALLER", "Database schema installer", _databaseSchemaInstaller),
            CreateDependencyCheck("AUDIT_ENGINE", "Audit engine", _auditService),
            CreateDependencyCheck("ALERT_ENGINE", "Alert engine", _alertService),
            CreateDependencyCheck("DASHBOARD_METRICS", "Dashboard metrics", _dashboardMetricsService)
        };

        checks.Add(await CreateDatabaseCheckAsync(cancellationToken).ConfigureAwait(false));

        string status = checks.Any(check => check.Status == "FAILED")
            ? "NOT_READY"
            : checks.Any(check => check.Status == "WARNING")
                ? "READY_WITH_WARNINGS"
                : "READY";

        var dto = new MvpReadinessDto(status, _clock.UtcNow, checks);
        return Result<MvpReadinessDto>.Ok(dto, "SmartWMS backend MVP readiness check completed.");
    }

    private MvpReadinessCheckDto CreateDependencyCheck(string code, string name, object? dependency)
    {
        return new MvpReadinessCheckDto(
            code,
            name,
            dependency is not null ? "OK" : "FAILED",
            dependency is not null ? "Dependency resolved." : "Dependency could not be resolved.",
            _clock.UtcNow,
            0);
    }

    private async Task<MvpReadinessCheckDto> CreateDatabaseCheckAsync(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            DatabaseHealthResult result = await _databaseHealthCheck.CheckAsync(cancellationToken).ConfigureAwait(false);
            stopwatch.Stop();

            return new MvpReadinessCheckDto(
                "DATABASE_CONNECTION",
                "Database connection",
                result.Success ? "OK" : "WARNING",
                result.Message,
                result.CheckedAtUtc,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            return new MvpReadinessCheckDto(
                "DATABASE_CONNECTION",
                "Database connection",
                "WARNING",
                $"Database check could not be completed: {ex.Message}",
                _clock.UtcNow,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
