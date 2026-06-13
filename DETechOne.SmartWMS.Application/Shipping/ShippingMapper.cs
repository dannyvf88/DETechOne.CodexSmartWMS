using DETechOne.SmartWMS.Contracts.Dtos.Shipping;
using DETechOne.SmartWMS.Domain.Entities.Shipping;

namespace DETechOne.SmartWMS.Application.Shipping;

public static class ShippingMapper
{
    public static ShippingDocumentDto ToDto(ShippingDocument shipping) => new()
    {
        Id = shipping.Id,
        ShippingNumber = shipping.ShippingNumber,
        PackingId = shipping.PackingId,
        PackingNumber = shipping.PackingNumber,
        WarehouseCode = shipping.WarehouseCode,
        CustomerCode = shipping.CustomerCode,
        CustomerName = shipping.CustomerName,
        RequestedBy = shipping.RequestedBy,
        Status = shipping.Status.ToString(),
        PackedQuantity = shipping.PackedQuantity,
        CreatedAtUtc = shipping.CreatedAtUtc,
        ConfirmedAtUtc = shipping.ConfirmedAtUtc,
        ConfirmedBy = shipping.ConfirmedBy,
        DeliveryDocEntry = shipping.DeliveryDocEntry,
        DeliveryDocNum = shipping.DeliveryDocNum,
        DeliveryCreatedAtUtc = shipping.DeliveryCreatedAtUtc,
        DeliveryCreatedBy = shipping.DeliveryCreatedBy,
        CancelledAtUtc = shipping.CancelledAtUtc,
        CancelledBy = shipping.CancelledBy,
        CancelReason = shipping.CancelReason,
        Lines = shipping.Lines.OrderBy(line => line.LineNumber).Select(ToDto).ToArray()
    };

    private static ShippingLineDto ToDto(ShippingLine line) => new()
    {
        LineNumber = line.LineNumber,
        ItemCode = line.ItemCode,
        WarehouseCode = line.WarehouseCode,
        LocationCode = line.LocationCode,
        PackedQuantity = line.PackedQuantity,
        LotNumber = line.LotNumber,
        UomCode = line.UomCode,
        Status = line.Status.ToString(),
        ConfirmedAtUtc = line.ConfirmedAtUtc,
        ConfirmedBy = line.ConfirmedBy
    };
}
