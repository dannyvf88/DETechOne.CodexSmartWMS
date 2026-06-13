namespace DETechOne.SmartWMS.Contracts.Requests.SAP;

public sealed class CreateSapInventoryTransferRequest
{
    public string FromWarehouseCode { get; init; } = string.Empty;
    public string ToWarehouseCode { get; init; } = string.Empty;
    public string Comments { get; init; } = string.Empty;
    public IReadOnlyCollection<CreateSapInventoryTransferLineRequest> Lines { get; init; } = Array.Empty<CreateSapInventoryTransferLineRequest>();
}
