namespace DETechOne.SmartWMS.Contracts.Dtos.Movement;

public sealed class MovementLineDto
{
    public Guid Id { get; init; }
    public int LineNumber { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string FromWarehouseCode { get; init; } = string.Empty;
    public string FromLocationCode { get; init; } = string.Empty;
    public string ToWarehouseCode { get; init; } = string.Empty;
    public string ToLocationCode { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public string? LotNumber { get; init; }
    public string? UomCode { get; init; }
}
