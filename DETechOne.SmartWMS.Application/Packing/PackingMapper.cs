using DETechOne.SmartWMS.Contracts.Dtos.Packing;
using DETechOne.SmartWMS.Domain.Entities.Packing;

namespace DETechOne.SmartWMS.Application.Packing;

internal static class PackingMapper
{
    public static PackingDocumentDto ToDto(PackingDocument packing)
    {
        return new PackingDocumentDto(
            packing.Id,
            packing.PackingNumber,
            packing.PickingId,
            packing.PickingNumber,
            packing.WarehouseCode,
            packing.RequestedBy,
            packing.Status,
            packing.PickedQuantity,
            packing.PackedQuantity,
            packing.PendingQuantity,
            packing.CreatedAtUtc,
            packing.StartedAtUtc,
            packing.CompletedAtUtc,
            packing.CancelledAtUtc,
            packing.Lines.OrderBy(line => line.LineNumber).Select(ToDto).ToArray());
    }

    private static PackingLineDto ToDto(PackingLine line)
    {
        return new PackingLineDto(
            line.Id,
            line.LineNumber,
            line.ItemCode,
            line.WarehouseCode,
            line.LocationCode,
            line.PickedQuantity,
            line.PackedQuantity,
            line.PendingQuantity,
            line.LotNumber,
            line.UomCode,
            line.PackageCode,
            line.Status);
    }
}
