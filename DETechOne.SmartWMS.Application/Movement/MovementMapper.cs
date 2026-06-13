using DETechOne.SmartWMS.Contracts.Dtos.Movement;
using DETechOne.SmartWMS.Domain.Entities.Movement;

namespace DETechOne.SmartWMS.Application.Movement;

public static class MovementMapper
{
    public static MovementDocumentDto ToDto(MovementDocument document)
    {
        return new MovementDocumentDto
        {
            Id = document.Id,
            MovementNumber = document.MovementNumber,
            MovementType = document.MovementType.ToString(),
            Status = document.Status.ToString(),
            ReferenceType = document.ReferenceType,
            ReferenceNumber = document.ReferenceNumber,
            RequestedBy = document.RequestedBy,
            CreatedAtUtc = document.CreatedAtUtc,
            ConfirmedAtUtc = document.ConfirmedAtUtc,
            ConfirmedBy = document.ConfirmedBy,
            CancelledAtUtc = document.CancelledAtUtc,
            CancelledBy = document.CancelledBy,
            CancelReason = document.CancelReason,
            Lines = document.Lines.OrderBy(line => line.LineNumber).Select(ToDto).ToArray()
        };
    }

    public static MovementLineDto ToDto(MovementLine line)
    {
        return new MovementLineDto
        {
            Id = line.Id,
            LineNumber = line.LineNumber,
            ItemCode = line.ItemCode,
            FromWarehouseCode = line.FromWarehouseCode,
            FromLocationCode = line.FromLocationCode,
            ToWarehouseCode = line.ToWarehouseCode,
            ToLocationCode = line.ToLocationCode,
            Quantity = line.Quantity,
            LotNumber = line.LotNumber,
            UomCode = line.UomCode
        };
    }
}
