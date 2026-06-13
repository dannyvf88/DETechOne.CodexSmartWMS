using DETechOne.SmartWMS.Infrastructure.Configuration.Database;

namespace DETechOne.SmartWMS.Tests.Infrastructure;

public sealed class DatabaseOptionsTests
{
    [Fact]
    public void IsConfigured_Should_Return_False_When_ConnectionString_Is_Empty()
    {
        var options = new DatabaseOptions { ConnectionString = string.Empty };

        Assert.False(options.IsConfigured);
    }

    [Fact]
    public void IsConfigured_Should_Return_True_When_ConnectionString_Has_Value()
    {
        var options = new DatabaseOptions { ConnectionString = "Server=.;Database=Test;Trusted_Connection=True;" };

        Assert.True(options.IsConfigured);
    }
}
