using DETechOne.SmartWMS.Application.Schema;
using DETechOne.SmartWMS.Contracts.Dtos.Schema;

namespace DETechOne.SmartWMS.Installer.Definitions.Schema;

public sealed class SmartWmsSchemaDefinitionProvider : IDatabaseSchemaDefinitionProvider
{
    public SchemaInstallPlanDto GetPlan()
    {
        IReadOnlyList<SchemaTableDefinitionDto> tables = new[]
        {
            DocumentHeader("@DTO_SW_OJ_HPICKING", "SmartWMS Picking Header", "Encabezado operativo de picking"),
            DocumentLine("@DTO_SW_OJ_LPICKING", "SmartWMS Picking Lines", "Detalle operativo de picking"),
            DocumentHeader("@DTO_SW_OJ_HPACKING", "SmartWMS Packing Header", "Encabezado operativo de packing"),
            DocumentLine("@DTO_SW_OJ_LPACKING", "SmartWMS Packing Lines", "Detalle operativo de packing"),
            DocumentHeader("@DTO_SW_OJ_HSHIPPING", "SmartWMS Shipping Header", "Encabezado operativo de embarque"),
            DocumentLine("@DTO_SW_OJ_LSHIPPING", "SmartWMS Shipping Lines", "Detalle operativo de embarque"),
            DocumentHeader("@DTO_SW_OJ_HMOVEMENT", "SmartWMS Movement Header", "Encabezado operativo de movimientos internos"),
            DocumentLine("@DTO_SW_OJ_LMOVEMENT", "SmartWMS Movement Lines", "Detalle operativo de movimientos internos"),
            InventoryTable("@DTO_SW_DT_INVLTE", "SmartWMS Inventory Lot", "Inventario por lote"),
            InventoryTable("@DTO_SW_DT_INVLCTN", "SmartWMS Inventory Location", "Inventario por ubicacion"),
            InventoryTable("@DTO_SW_DT_INVRSV", "SmartWMS Inventory Reservation", "Reservas de inventario"),
            DocumentHeader("@DTO_SW_OJ_HLOGUNIT", "SmartWMS Logistic Unit", "Unidades logisticas y contenedores"),
            InventoryTable("@DTO_SW_ESQ_HDR", "SmartWMS Stock Snapshot Header", "Encabezado de snapshot de existencias"),
            InventoryTable("@DTO_SW_ESQ_ITM", "SmartWMS Stock Snapshot Item", "Inventario consolidado por articulo"),
            InventoryTable("@DTO_SW_ESQ_DET", "SmartWMS Stock Snapshot Detail", "Detalle de snapshot de existencias"),
            BarcodeTable(),
            ConfigTable(),
            AuditTable(),
            DeviceTable()
        };

        IReadOnlyList<SchemaUdoDefinitionDto> udos = new[]
        {
            Udo("DTO_SW_PICKING", "SmartWMS Picking", "DTO_SW_OJ_HPICKING", "Document", new[] { new SchemaUdoChildTableDefinitionDto("DTO_SW_OJ_LPICKING", "Lines") }),
            Udo("DTO_SW_PACKING", "SmartWMS Packing", "DTO_SW_OJ_HPACKING", "Document", new[] { new SchemaUdoChildTableDefinitionDto("DTO_SW_OJ_LPACKING", "Lines") }),
            Udo("DTO_SW_SHIPPING", "SmartWMS Shipping", "DTO_SW_OJ_HSHIPPING", "Document", new[] { new SchemaUdoChildTableDefinitionDto("DTO_SW_OJ_LSHIPPING", "Lines") }),
            Udo("DTO_SW_MOVEMENT", "SmartWMS Movement", "DTO_SW_OJ_HMOVEMENT", "Document", new[] { new SchemaUdoChildTableDefinitionDto("DTO_SW_OJ_LMOVEMENT", "Lines") }),
            Udo("DTO_SW_LOGUNIT", "SmartWMS Logistic Unit", "DTO_SW_OJ_HLOGUNIT", "MasterData", Array.Empty<SchemaUdoChildTableDefinitionDto>())
        };

        IReadOnlyList<SchemaPermissionDefinitionDto> permissions = new[]
        {
            Permission("SWMS_ADMIN", "SmartWMS Administrator", "Administracion total de SmartWMS", "SmartWMS", "Full"),
            Permission("SWMS_PICKING", "SmartWMS Picking", "Operacion de picking", "SmartWMS", "ReadOnly"),
            Permission("SWMS_PACKING", "SmartWMS Packing", "Operacion de packing", "SmartWMS", "ReadOnly"),
            Permission("SWMS_SHIPPING", "SmartWMS Shipping", "Operacion de shipping y entregas", "SmartWMS", "ReadOnly"),
            Permission("SWMS_INVENTORY", "SmartWMS Inventory", "Consulta y ajustes de inventario", "SmartWMS", "ReadOnly"),
            Permission("SWMS_METADATA", "SmartWMS Metadata Installer", "Instalacion y actualizacion de metadata", "SmartWMS", "None")
        };

        IReadOnlyList<SchemaMenuDefinitionDto> menus = new[]
        {
            new SchemaMenuDefinitionDto("SWMS_ROOT", "SmartWMS", "43520", 50, null, "SWMS_ADMIN"),
            new SchemaMenuDefinitionDto("SWMS_PICKING", "Picking", "SWMS_ROOT", 10, "DTO_SW_PICKING", "SWMS_PICKING"),
            new SchemaMenuDefinitionDto("SWMS_PACKING", "Packing", "SWMS_ROOT", 20, "DTO_SW_PACKING", "SWMS_PACKING"),
            new SchemaMenuDefinitionDto("SWMS_SHIPPING", "Shipping", "SWMS_ROOT", 30, "DTO_SW_SHIPPING", "SWMS_SHIPPING"),
            new SchemaMenuDefinitionDto("SWMS_MOVEMENT", "Movimientos", "SWMS_ROOT", 40, "DTO_SW_MOVEMENT", "SWMS_INVENTORY"),
            new SchemaMenuDefinitionDto("SWMS_SETUP", "Configuracion", "SWMS_ROOT", 90, null, "SWMS_METADATA")
        };

        IReadOnlyList<SchemaSeedDataDefinitionDto> seedData = new[]
        {
            Seed("@DTO_SW_CFG", "VERSION", "SmartWMS Version", new Dictionary<string, string> { ["U_Value"] = "10.10", ["U_Description"] = "Database Schema Installer Real" }),
            Seed("@DTO_SW_CFG", "DEFAULT_STATUS", "Default Document Status", new Dictionary<string, string> { ["U_Value"] = "OP", ["U_Description"] = "OP=Open;TE=Terminated;CA=Cancelled" }),
            Seed("@DTO_SW_CFG", "DEVICE_HEARTBEAT_SECONDS", "Device Heartbeat", new Dictionary<string, string> { ["U_Value"] = "60", ["U_Description"] = "Frecuencia esperada de heartbeat de dispositivos" })
        };

        return new SchemaInstallPlanDto("10.10", tables, udos, permissions, menus, seedData);
    }

    private static SchemaTableDefinitionDto DocumentHeader(string code, string name, string description) =>
        new(code, name, "bott_Document", description, new[]
        {
            Field("U_Status", "Estado", "db_Alpha", 2, mandatory: true, validValues: StatusValues()),
            Field("U_Company", "Compania", "db_Alpha", 30, mandatory: true),
            Field("U_WhsCode", "Almacen", "db_Alpha", 20),
            Field("U_BaseEntry", "Documento base", "db_Numeric", 11),
            Field("U_BaseType", "Tipo documento base", "db_Numeric", 11),
            Field("U_DeviceId", "Dispositivo", "db_Alpha", 50),
            Field("U_UserCode", "Usuario operativo", "db_Alpha", 50),
            Field("U_StartedAt", "Fecha/hora inicio", "db_Date"),
            Field("U_FinishedAt", "Fecha/hora fin", "db_Date"),
            Field("U_Remarks", "Comentarios", "db_Memo")
        });

    private static SchemaTableDefinitionDto DocumentLine(string code, string name, string description) =>
        new(code, name, "bott_DocumentLines", description, new[]
        {
            Field("U_HeaderCode", "Codigo encabezado", "db_Alpha", 50, mandatory: true),
            Field("U_LineNum", "Numero de linea", "db_Numeric", 11, mandatory: true),
            Field("U_ItemCode", "Articulo", "db_Alpha", 50, mandatory: true, linkedTable: "OITM"),
            Field("U_ItemName", "Descripcion articulo", "db_Alpha", 200),
            Field("U_FromWhsCode", "Almacen origen", "db_Alpha", 20, linkedTable: "OWHS"),
            Field("U_ToWhsCode", "Almacen destino", "db_Alpha", 20, linkedTable: "OWHS"),
            Field("U_BinCode", "Ubicacion", "db_Alpha", 50),
            Field("U_BatchNumber", "Lote", "db_Alpha", 100),
            Field("U_Quantity", "Cantidad", "db_Float"),
            Field("U_BaseUoMQty", "Cantidad unidad base", "db_Float"),
            Field("U_Status", "Estado", "db_Alpha", 2, mandatory: true, validValues: StatusValues())
        });

    private static SchemaTableDefinitionDto InventoryTable(string code, string name, string description) =>
        new(code, name, "bott_NoObject", description, new[]
        {
            Field("U_Company", "Compania", "db_Alpha", 30, mandatory: true),
            Field("U_WhsCode", "Almacen", "db_Alpha", 20, mandatory: true, linkedTable: "OWHS"),
            Field("U_ItemCode", "Articulo", "db_Alpha", 50, mandatory: true, linkedTable: "OITM"),
            Field("U_BinCode", "Ubicacion", "db_Alpha", 50),
            Field("U_BatchNumber", "Lote", "db_Alpha", 100),
            Field("U_OnHand", "Existencia", "db_Float"),
            Field("U_Committed", "Comprometido", "db_Float"),
            Field("U_Available", "Disponible", "db_Float"),
            Field("U_LastSyncAt", "Ultima sincronizacion", "db_Date")
        });

    private static SchemaTableDefinitionDto BarcodeTable() =>
        new("@DTO_SW_ESQ_BRC", "SmartWMS Barcode Map", "bott_NoObject", "Mapa de codigos de barra", new[]
        {
            Field("U_ItemCode", "Articulo", "db_Alpha", 50, mandatory: true, linkedTable: "OITM"),
            Field("U_BarCode", "Codigo de barras", "db_Alpha", 100, mandatory: true),
            Field("U_UomCode", "Unidad de medida", "db_Alpha", 20),
            Field("U_BaseQty", "Cantidad base", "db_Float"),
            Field("U_IsActive", "Activo", "db_Alpha", 1, defaultValue: "Y", validValues: YesNoValues())
        });

    private static SchemaTableDefinitionDto ConfigTable() =>
        new("@DTO_SW_CFG", "SmartWMS Configuration", "bott_NoObject", "Configuracion inicial SmartWMS", new[]
        {
            Field("U_Value", "Valor", "db_Alpha", 254),
            Field("U_Description", "Descripcion", "db_Alpha", 254),
            Field("U_IsEncrypted", "Encriptado", "db_Alpha", 1, defaultValue: "N", validValues: YesNoValues())
        });

    private static SchemaTableDefinitionDto AuditTable() =>
        new("@DTO_SW_AUDIT", "SmartWMS Audit", "bott_NoObject", "Bitacora de auditoria SmartWMS", new[]
        {
            Field("U_EventType", "Tipo evento", "db_Alpha", 50, mandatory: true),
            Field("U_Source", "Origen", "db_Alpha", 50),
            Field("U_UserCode", "Usuario", "db_Alpha", 50),
            Field("U_DeviceId", "Dispositivo", "db_Alpha", 50),
            Field("U_Message", "Mensaje", "db_Memo"),
            Field("U_CreatedAt", "Fecha evento", "db_Date")
        });

    private static SchemaTableDefinitionDto DeviceTable() =>
        new("@DTO_SW_DEVICE", "SmartWMS Device", "bott_MasterData", "Catalogo de dispositivos SmartWMS", new[]
        {
            Field("U_DeviceType", "Tipo dispositivo", "db_Alpha", 30),
            Field("U_DeviceName", "Nombre dispositivo", "db_Alpha", 100),
            Field("U_LastHeartbeat", "Ultimo heartbeat", "db_Date"),
            Field("U_IsActive", "Activo", "db_Alpha", 1, defaultValue: "Y", validValues: YesNoValues())
        });

    private static SchemaUdoDefinitionDto Udo(string code, string name, string mainTableCode, string objectType, IReadOnlyList<SchemaUdoChildTableDefinitionDto> childTables) =>
        new(code, name, mainTableCode, objectType, true, true, true, false, true, true,
            new[] { "DocEntry", "DocNum", "U_Status", "U_WhsCode", "U_UserCode" }, childTables);

    private static SchemaPermissionDefinitionDto Permission(string code, string name, string description, string category, string defaultAuthorization) =>
        new(code, name, description, category, defaultAuthorization);

    private static SchemaSeedDataDefinitionDto Seed(string tableCode, string code, string name, IReadOnlyDictionary<string, string> values) =>
        new(tableCode, code, name, values);

    private static SchemaFieldDefinitionDto Field(
        string name,
        string description,
        string sapType,
        int size = 0,
        int editSize = 0,
        bool mandatory = false,
        string? defaultValue = null,
        string? linkedTable = null,
        IReadOnlyDictionary<string, string>? validValues = null) =>
        new(name, description, sapType, size, editSize, mandatory, defaultValue, linkedTable, validValues);

    private static IReadOnlyDictionary<string, string> StatusValues() => new Dictionary<string, string>
    {
        ["OP"] = "Open",
        ["TE"] = "Terminated",
        ["CA"] = "Cancelled"
    };

    private static IReadOnlyDictionary<string, string> YesNoValues() => new Dictionary<string, string>
    {
        ["Y"] = "Yes",
        ["N"] = "No"
    };
}
