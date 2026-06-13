using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.Domain.Entities;

public sealed class Item : BaseEntity
{
    private Item()
    {
        ItemCode = string.Empty;
        ItemName = string.Empty;
    }

    public Item(string itemCode, string itemName)
    {
        ItemCode = string.IsNullOrWhiteSpace(itemCode) ? throw new ArgumentException("Item code is required.", nameof(itemCode)) : itemCode.Trim();
        ItemName = string.IsNullOrWhiteSpace(itemName) ? throw new ArgumentException("Item name is required.", nameof(itemName)) : itemName.Trim();
    }

    public string ItemCode { get; private set; }
    public string ItemName { get; private set; }
}
