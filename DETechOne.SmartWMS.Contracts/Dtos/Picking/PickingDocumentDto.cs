namespace DETechOne.SmartWMS.Contracts.Dtos.Picking;

public sealed class PickingDocumentDto
{
    public Guid Id { get; set; }
    public string PickingNumber { get; set; } = string.Empty;
    public string SourceDocumentType { get; set; } = string.Empty;
    public string SourceDocumentNumber { get; set; } = string.Empty;
    public string WarehouseCode { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal RequiredQuantity { get; set; }
    public decimal PickedQuantity { get; set; }
    public decimal PendingQuantity { get; set; }
    public DateTime? StartedAtUtc { get; set; }
    public string? StartedBy { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string? CompletedBy { get; set; }
    public DateTime? CancelledAtUtc { get; set; }
    public string? CancelledBy { get; set; }
    public string? CancelReason { get; set; }
    public IReadOnlyList<PickingLineDto> Lines { get; set; } = Array.Empty<PickingLineDto>();
}
