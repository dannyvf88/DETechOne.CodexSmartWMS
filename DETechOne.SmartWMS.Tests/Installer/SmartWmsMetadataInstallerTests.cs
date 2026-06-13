using DETechOne.SmartWMS.Application.Common;
using DETechOne.SmartWMS.Application.Metadata;
using DETechOne.SmartWMS.Contracts.Dtos.Metadata;
using DETechOne.SmartWMS.Contracts.Requests.Metadata;
using DETechOne.SmartWMS.Installer.Definitions;
using DETechOne.SmartWMS.Installer.Services;

namespace DETechOne.SmartWMS.Tests.Installer;

public sealed class SmartWmsMetadataInstallerTests
{
    [Fact]
    public async Task InstallAsync_WhenDryRunIsTrue_ShouldNotCallSapCreateOrUpdate()
    {
        var sapService = new FakeSapMetadataService();
        var installer = new SmartWmsMetadataInstaller(
            new SmartWmsMetadataDefinitionProvider(),
            sapService,
            new FixedClock());

        var result = await installer.InstallAsync(new RunMetadataInstallRequest("SBODEMO", DryRun: true));

        Assert.True(result.Success);
        Assert.True(result.DryRun);
        Assert.Equal(0, sapService.CreateOrUpdateCalls);
        Assert.Equal(result.TotalSteps, result.SuccessfulSteps);
    }

    private sealed class FakeSapMetadataService : ISapMetadataService
    {
        public int CreateOrUpdateCalls { get; private set; }

        public Task<bool> ExistsAsync(MetadataObjectDefinitionDto definition, string companyCode, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task CreateOrUpdateAsync(MetadataObjectDefinitionDto definition, string companyCode, CancellationToken cancellationToken = default)
        {
            CreateOrUpdateCalls++;
            return Task.CompletedTask;
        }
    }

    private sealed class FixedClock : IClock
    {
        public DateTime UtcNow { get; } = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
