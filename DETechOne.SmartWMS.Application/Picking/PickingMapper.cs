using DETechOne.SmartWMS.Contracts.Dtos.Picking;
using DETechOne.SmartWMS.Domain.Entities.Picking;

namespace DETechOne.SmartWMS.Application.Picking;

internal static class PickingMapper
{
    public static PickingDocumentDto ToDto(PickingDocument document)
    {
        return new PickingDocumentDto
        {
            Id = document.Id,
            PickingNumber = document.PickingNumber,
            SourceDocumentType = document.SourceDocumentType,
            SourceDocumentNumber = document.SourceDocumentNumber,
            WarehouseCode = document.WarehouseCode,
            RequestedBy = document.RequestedBy,
            Status = document.Status.ToString(),
            RequiredQuantity = document.RequiredQuantity,
            PickedQuantity = document.PickedQuantity,
            PendingQuantity = document.PendingQuantity,
            StartedAtUtc = document.StartedAtUtc,
            StartedBy = document.StartedBy,
            CompletedAtUtc = document.CompletedAtUtc,
            CompletedBy = document.CompletedBy,
            CancelledAtUtc = document.CancelledAtUtc,
            CancelledBy = document.CancelledBy,
            CancelReason = document.CancelReason,
            Lines = document.Lines
                .OrderBy(line => line.LineNumber)
                .Select(ToDto)
                .ToArray()
        };
    }

    private static PickingLineDto ToDto(PickingLine line)
    {
        return new PickingLineDto
        {
            Id = line.Id,
            LineNumber = line.LineNumber,
            ItemCode = line.ItemCode,
            WarehouseCode = line.WarehouseCode,
            LocationCode = line.LocationCode,
            RequiredQuantity = line.RequiredQuantity,
            PickedQuantity = line.PickedQuantity,
            PendingQuantity = line.PendingQuantity,
            LotNumber = line.LotNumber,
            UomCode = line.UomCode,
            Status = line.Status.ToString()
        };
    }
}
