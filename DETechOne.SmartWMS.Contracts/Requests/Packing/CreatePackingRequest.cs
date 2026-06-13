namespace DETechOne.SmartWMS.Contracts.Requests.Packing;

public sealed class CreatePackingRequest
{
    public Guid PickingId { get; set; }
    public string PickingNumber { get; set; } = string.Empty;
    public string WarehouseCode { get; set; } = string.Empty;
    public List<CreatePackingLineRequest> Lines { get; set; } = new();
}
