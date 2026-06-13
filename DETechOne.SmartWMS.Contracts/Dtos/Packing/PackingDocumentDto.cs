using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Contracts.Dtos.Packing;

public sealed record PackingDocumentDto(
    Guid Id,
    string PackingNumber,
    Guid PickingId,
    string PickingNumber,
    string WarehouseCode,
    string RequestedBy,
    PackingStatus Status,
    decimal PickedQuantity,
    decimal PackedQuantity,
    decimal PendingQuantity,
    DateTime CreatedAtUtc,
    DateTime? StartedAtUtc,
    DateTime? CompletedAtUtc,
    DateTime? CancelledAtUtc,
    IReadOnlyList<PackingLineDto> Lines);
