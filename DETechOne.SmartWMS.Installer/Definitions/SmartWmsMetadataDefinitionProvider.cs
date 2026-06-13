using DETechOne.SmartWMS.Application.Metadata;
using DETechOne.SmartWMS.Contracts.Dtos.Metadata;
using DETechOne.SmartWMS.Domain.Enums;

namespace DETechOne.SmartWMS.Installer.Definitions;

public sealed class SmartWmsMetadataDefinitionProvider : IMetadataDefinitionProvider
{
    public IReadOnlyList<MetadataObjectDefinitionDto> GetDefinitions()
    {
        return new List<MetadataObjectDefinitionDto>
        {
            CreateDocumentTable("@DTO_SW_OJ_HPICKING", "SmartWMS Picking Header", "Encabezado operativo de picking"),
            CreateLineTable("@DTO_SW_OJ_LPICKING", "SmartWMS Picking Lines", "Detalle operativo de picking"),
            CreateDocumentTable("@DTO_SW_OJ_HPACKING", "SmartWMS Packing Header", "Encabezado operativo de packing"),
            CreateLineTable("@DTO_SW_OJ_LPACKING", "SmartWMS Packing Lines", "Detalle operativo de packing"),
            CreateInventoryTable("@DTO_SW_DT_INVLTE", "SmartWMS Inventory Lot", "Inventario por lote"),
            CreateInventoryTable("@DTO_SW_DT_INVLCTN", "SmartWMS Inventory Location", "Inventario por ubicacion"),
            CreateDocumentTable("@DTO_SW_OJ_HLOGUNIT", "SmartWMS Logistic Unit", "Unidades logisticas y contenedores"),
            CreateInventoryTable("@DTO_SW_ESQ_HDR", "SmartWMS Stock Snapshot Header", "Encabezado de snapshot de existencias"),
            CreateInventoryTable("@DTO_SW_ESQ_ITM", "SmartWMS Stock Snapshot Item", "Inventario consolidado por articulo"),
            CreateInventoryTable("@DTO_SW_ESQ_DET", "SmartWMS Stock Snapshot Detail", "Detalle de snapshot de existencias"),
            CreateInventoryTable("@DTO_SW_ESQ_BRC", "SmartWMS Barcode Map", "Mapa de codigos de barra"),
            CreateUdo("DTO_SW_PICKING", "SmartWMS Picking UDO", "UDO principal para picking"),
            CreateUdo("DTO_SW_PACKING", "SmartWMS Packing UDO", "UDO principal para packing"),
            CreateUdo("DTO_SW_LOGUNIT", "SmartWMS Logistic Unit UDO", "UDO para unidades logisticas"),
            CreatePermission("SWMS_SECURITY_BASE", "SmartWMS Security Base", "Permisos base de SmartWMS"),
        };
    }

    private static MetadataObjectDefinitionDto CreateDocumentTable(string code, string name, string description)
    {
        return new MetadataObjectDefinitionDto(code, name, MetadataObjectType.UserTable, description, new List<MetadataFieldDefinitionDto>
        {
            new("U_Status", "Estado", "Alphanumeric", 2, true, "OP=Open;TE=Terminated;CA=Cancelled"),
            new("U_Company", "Compania", "Alphanumeric", 30, true),
            new("U_WhsCode", "Almacen", "Alphanumeric", 20),
            new("U_BaseEntry", "Documento base", "Numeric", 11),
            new("U_BaseType", "Tipo documento base", "Numeric", 11),
            new("U_DeviceId", "Dispositivo", "Alphanumeric", 50),
            new("U_UserCode", "Usuario operativo", "Alphanumeric", 50),
            new("U_StartedAt", "Fecha/hora inicio", "DateTime", 0),
            new("U_FinishedAt", "Fecha/hora fin", "DateTime", 0),
            new("U_Remarks", "Comentarios", "Memo", 0)
        });
    }

    private static MetadataObjectDefinitionDto CreateLineTable(string code, string name, string description)
    {
        return new MetadataObjectDefinitionDto(code, name, MetadataObjectType.UserTable, description, new List<MetadataFieldDefinitionDto>
        {
            new("U_HeaderCode", "Codigo encabezado", "Alphanumeric", 50, true),
            new("U_LineNum", "Numero de linea", "Numeric", 11, true),
            new("U_ItemCode", "Articulo", "Alphanumeric", 50, true),
            new("U_ItemName", "Descripcion articulo", "Alphanumeric", 200),
            new("U_BinCode", "Ubicacion", "Alphanumeric", 50),
            new("U_BatchNumber", "Lote", "Alphanumeric", 100),
            new("U_Quantity", "Cantidad", "Quantity", 0),
            new("U_BaseUoMQty", "Cantidad unidad base", "Quantity", 0),
            new("U_Status", "Estado", "Alphanumeric", 2, true, "OP=Open;TE=Terminated;CA=Cancelled")
        });
    }

    private static MetadataObjectDefinitionDto CreateInventoryTable(string code, string name, string description)
    {
        return new MetadataObjectDefinitionDto(code, name, MetadataObjectType.UserTable, description, new List<MetadataFieldDefinitionDto>
        {
            new("U_Company", "Compania", "Alphanumeric", 30, true),
            new("U_WhsCode", "Almacen", "Alphanumeric", 20, true),
            new("U_ItemCode", "Articulo", "Alphanumeric", 50, true),
            new("U_BinCode", "Ubicacion", "Alphanumeric", 50),
            new("U_BatchNumber", "Lote", "Alphanumeric", 100),
            new("U_OnHand", "Existencia", "Quantity", 0),
            new("U_Committed", "Comprometido", "Quantity", 0),
            new("U_Available", "Disponible", "Quantity", 0),
            new("U_LastSyncAt", "Ultima sincronizacion", "DateTime", 0)
        });
    }

    private static MetadataObjectDefinitionDto CreateUdo(string code, string name, string description)
    {
        return new MetadataObjectDefinitionDto(code, name, MetadataObjectType.UserObject, description, Array.Empty<MetadataFieldDefinitionDto>());
    }

    private static MetadataObjectDefinitionDto CreatePermission(string code, string name, string description)
    {
        return new MetadataObjectDefinitionDto(code, name, MetadataObjectType.Permission, description, Array.Empty<MetadataFieldDefinitionDto>());
    }
}
