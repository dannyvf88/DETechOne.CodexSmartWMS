using DETechOne.SmartWMS.Application.Alerts;
using DETechOne.SmartWMS.Application.Audit;
using DETechOne.SmartWMS.Application.Common;
using DETechOne.SmartWMS.Application.Dashboard;
using DETechOne.SmartWMS.Application.EndToEnd;
using DETechOne.SmartWMS.Application.Inventory;
using DETechOne.SmartWMS.Application.Movement;
using DETechOne.SmartWMS.Application.Picking;
using DETechOne.SmartWMS.Application.Packing;
using DETechOne.SmartWMS.Application.Shipping;
using DETechOne.SmartWMS.Application.Stabilization;
using Microsoft.Extensions.DependencyInjection;

namespace DETechOne.SmartWMS.Application.Extensions;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddSmartWmsApplication(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IEndToEndFlowStateStore, InMemoryEndToEndFlowStateStore>();
        services.AddScoped<IWarehouseOperationFlowService, WarehouseOperationFlowService>();
        services.AddScoped<IEndToEndFlowOrchestrator, OrderToDeliveryFlowOrchestrator>();
        services.AddScoped<IDashboardMetricsService, DashboardMetricsService>();
        services.AddSingleton<IDeviceMetricsReader, NullDeviceMetricsReader>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IMovementService, MovementService>();
        services.AddScoped<IPickingService, PickingService>();
        services.AddScoped<IPackingService, PackingService>();
        services.AddScoped<IShippingService, ShippingService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IAlertService, AlertService>();
        services.AddScoped<IMvpReadinessService, MvpReadinessService>();
        return services;
    }
}
