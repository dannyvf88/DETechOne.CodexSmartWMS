using DETechOne.SmartWMS.Domain.Entities;

namespace DETechOne.SmartWMS.Tests.Domain;

public sealed class WarehouseTests
{
    [Fact]
    public void Constructor_WhenCodeIsEmpty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Warehouse(string.Empty, "Main"));
    }

    [Fact]
    public void Constructor_WhenValuesAreValid_CreatesWarehouse()
    {
        var warehouse = new Warehouse("01", "Main Warehouse");

        Assert.Equal("01", warehouse.Code);
        Assert.Equal("Main Warehouse", warehouse.Name);
    }
}
