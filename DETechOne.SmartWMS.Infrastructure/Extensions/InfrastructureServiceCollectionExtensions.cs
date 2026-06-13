using DETechOne.SmartWMS.Application.Alerts;
using DETechOne.SmartWMS.Application.Audit;
using DETechOne.SmartWMS.Application.Inventory;
using DETechOne.SmartWMS.Application.Persistence;
using DETechOne.SmartWMS.Application.Movement;
using DETechOne.SmartWMS.Application.Picking;
using DETechOne.SmartWMS.Application.Packing;
using DETechOne.SmartWMS.Application.Shipping;
using DETechOne.SmartWMS.Application.Security;
using DETechOne.SmartWMS.Domain.Interfaces;
using DETechOne.SmartWMS.Infrastructure.Alerts;
using DETechOne.SmartWMS.Infrastructure.Audit;
using DETechOne.SmartWMS.Infrastructure.Configuration.Database;
using DETechOne.SmartWMS.Infrastructure.Inventory;
using DETechOne.SmartWMS.Infrastructure.Movement;
using DETechOne.SmartWMS.Infrastructure.Picking;
using DETechOne.SmartWMS.Infrastructure.Packing;
using DETechOne.SmartWMS.Infrastructure.Shipping;
using DETechOne.SmartWMS.Infrastructure.Persistence;
using DETechOne.SmartWMS.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DETechOne.SmartWMS.Infrastructure.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddSmartWmsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IOptions<DatabaseOptions>>(_ => Options.Create(BindDatabaseOptions(configuration)));

        services.AddScoped<IDbConnectionFactory, SmartWmsDbConnectionFactory>();
        services.AddScoped<IQueryExecutor, SqlQueryExecutor>();
        services.AddScoped<IDatabaseHealthCheck, DatabaseHealthCheck>();
        services.AddScoped<IUnitOfWork, SmartWmsUnitOfWork>();
        services.AddSingleton<IInventoryRepository, InMemoryInventoryRepository>();
        services.AddSingleton<IMovementRepository, InMemoryMovementRepository>();
        services.AddSingleton<IPickingRepository, InMemoryPickingRepository>();
        services.AddSingleton<IPackingRepository, InMemoryPackingRepository>();
        services.AddSingleton<IShippingRepository, InMemoryShippingRepository>();
        services.AddSingleton<IAuditLogRepository, InMemoryAuditLogRepository>();
        services.AddSingleton<IAlertRepository, InMemoryAlertRepository>();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, InMemoryAuthService>();

        return services;
    }

    private static DatabaseOptions BindDatabaseOptions(IConfiguration configuration)
    {
        IConfigurationSection section = configuration.GetSection(DatabaseOptions.SectionName);

        return new DatabaseOptions
        {
            Provider = section[nameof(DatabaseOptions.Provider)] ?? DatabaseProviderNames.SqlServer,
            ConnectionString = section[nameof(DatabaseOptions.ConnectionString)] ?? string.Empty,
            HanaProviderInvariantName = section[nameof(DatabaseOptions.HanaProviderInvariantName)] ?? "Sap.Data.Hana",
            CommandTimeoutSeconds = int.TryParse(section[nameof(DatabaseOptions.CommandTimeoutSeconds)], out int timeout) ? timeout : 30,
            EnableSensitiveDataLogging = bool.TryParse(section[nameof(DatabaseOptions.EnableSensitiveDataLogging)], out bool sensitiveLogging) && sensitiveLogging
        };
    }
}
