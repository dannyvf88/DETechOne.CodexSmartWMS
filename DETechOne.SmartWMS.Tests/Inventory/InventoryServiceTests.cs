using DETechOne.SmartWMS.Application.Inventory;
using DETechOne.SmartWMS.Contracts.Requests.Inventory;
using DETechOne.SmartWMS.Infrastructure.Inventory;

namespace DETechOne.SmartWMS.Tests.Inventory;

public sealed class InventoryServiceTests
{
    [Fact]
    public async Task AdjustAsync_WhenAdjustmentIsIn_IncreasesAvailableQuantity()
    {
        var repository = new InMemoryInventoryRepository();
        var service = new InventoryService(repository);

        var adjustResult = await service.AdjustAsync(new AdjustInventoryRequest
        {
            ItemCode = "A0001",
            WarehouseCode = "01",
            LocationCode = "A-01-01",
            Quantity = 10,
            AdjustmentType = "IN",
            ReasonCode = "INIT"
        }, "unit-test");

        var availabilityResult = await service.GetAvailabilityAsync(new GetInventoryAvailabilityRequest
        {
            ItemCode = "A0001",
            WarehouseCode = "01",
            RequestedQuantity = 5
        });

        Assert.True(adjustResult.Success);
        Assert.NotNull(adjustResult.Value);
        Assert.True(availabilityResult.Success);
        Assert.NotNull(availabilityResult.Value);
        Assert.Equal(10, availabilityResult.Value.AvailableQuantity);
        Assert.Equal("Available", availabilityResult.Value.Status);
    }

    [Fact]
    public async Task ReserveAsync_WhenEnoughInventory_ReservesQuantity()
    {
        var repository = new InMemoryInventoryRepository();
        var service = new InventoryService(repository);

        await service.AdjustAsync(new AdjustInventoryRequest
        {
            ItemCode = "A0001",
            WarehouseCode = "01",
            LocationCode = "A-01-01",
            Quantity = 10,
            AdjustmentType = "IN",
            ReasonCode = "INIT"
        }, "unit-test");

        var reserveResult = await service.ReserveAsync(new ReserveInventoryRequest
        {
            ItemCode = "A0001",
            WarehouseCode = "01",
            LocationCode = "A-01-01",
            Quantity = 4,
            ReferenceType = "PICKING",
            ReferenceNumber = "PK-001"
        }, "unit-test");

        var availabilityResult = await service.GetAvailabilityAsync(new GetInventoryAvailabilityRequest
        {
            ItemCode = "A0001",
            WarehouseCode = "01",
            RequestedQuantity = 6
        });

        Assert.True(reserveResult.Success);
        Assert.NotNull(reserveResult.Value);
        Assert.True(availabilityResult.Success);
        Assert.NotNull(availabilityResult.Value);
        Assert.Equal(6, availabilityResult.Value.AvailableQuantity);
        Assert.Equal(4, availabilityResult.Value.ReservedQuantity);
    }
}
