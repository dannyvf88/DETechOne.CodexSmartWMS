namespace DETechOne.SmartWMS.Contracts.Requests.SAP;

public sealed class CreateSapInventoryTransferLineRequest
{
    public int LineNumber { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public string? LotNumber { get; init; }
    public string? FromLocationCode { get; init; }
    public string? ToLocationCode { get; init; }
}
