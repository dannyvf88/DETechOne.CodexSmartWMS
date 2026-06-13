namespace DETechOne.SmartWMS.Contracts.Dtos.Shipping;

public sealed class SapDeliveryResultDto
{
    public int DocEntry { get; init; }
    public int DocNum { get; init; }
    public string Message { get; init; } = string.Empty;
}
