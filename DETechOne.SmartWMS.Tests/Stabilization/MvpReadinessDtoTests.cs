using DETechOne.SmartWMS.Contracts.Dtos.Stabilization;

namespace DETechOne.SmartWMS.Tests.Stabilization;

public sealed class MvpReadinessDtoTests
{
    [Fact]
    public void Counters_ShouldReturnExpectedTotals()
    {
        var checks = new[]
        {
            new MvpReadinessCheckDto("A", "Check A", "OK", "Ok", DateTime.UtcNow, 1),
            new MvpReadinessCheckDto("B", "Check B", "WARNING", "Warning", DateTime.UtcNow, 2),
            new MvpReadinessCheckDto("C", "Check C", "FAILED", "Failed", DateTime.UtcNow, 3)
        };

        var dto = new MvpReadinessDto("NOT_READY", DateTime.UtcNow, checks);

        Assert.Equal(3, dto.TotalChecks);
        Assert.Equal(1, dto.PassedChecks);
        Assert.Equal(1, dto.WarningChecks);
        Assert.Equal(1, dto.FailedChecks);
    }
}
