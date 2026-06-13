using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Contracts.Dtos.Packing;

public sealed record PackingLineDto(
    Guid Id,
    int LineNumber,
    string ItemCode,
    string WarehouseCode,
    string? LocationCode,
    decimal PickedQuantity,
    decimal PackedQuantity,
    decimal PendingQuantity,
    string? LotNumber,
    string? UomCode,
    string? PackageCode,
    PackingLineStatus Status);
