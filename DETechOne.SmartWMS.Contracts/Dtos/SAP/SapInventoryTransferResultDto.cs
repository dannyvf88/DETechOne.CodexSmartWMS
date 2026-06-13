namespace DETechOne.SmartWMS.Contracts.Dtos.SAP;

public sealed class SapInventoryTransferResultDto
{
    public int DocEntry { get; init; }
    public int DocNum { get; init; }
    public string Message { get; init; } = string.Empty;
}
