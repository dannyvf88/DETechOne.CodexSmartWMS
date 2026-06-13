# Fase 10.11.4 — SAP Sales Order Reader Real

## Objetivo

Conectar el flujo End-to-End con una lectura real de órdenes de venta de SAP Business One mediante Service Layer, sin introducir dependencia directa a `SAPbobsCOM` ni a la DI API durante compilación.

## Alcance implementado

- Implementación real de `SapSalesOrderReader`.
- Consumo de Service Layer mediante `IServiceLayerClient`.
- Lectura del endpoint:

```text
Orders({docEntry})?$expand=DocumentLines
```

- Mapeo robusto hacia:
  - `SapSalesOrderDto`
  - `SapSalesOrderLineDto`

## Campos mapeados

### Cabecera

- `DocEntry`
- `DocNum`
- `CardCode`
- `CardName`
- `DocumentStatus` / `DocStatus`
- `DocDate`

### Líneas

- `LineNum`
- `ItemCode`
- `ItemDescription` / `Dscription`
- `WarehouseCode` / `WhsCode`
- `Quantity`
- `OpenQuantity` / `RemainingOpenQuantity`
- `UoMCode` / `UomCode` / `MeasureUnit`

## Reglas importantes

1. Si `DocEntry <= 0`, se devuelve error de validación.
2. Si Service Layer no está configurado, se conserva el error controlado `SAP_NOT_CONFIGURED`.
3. Si SAP devuelve una respuesta inválida, se devuelve error de parseo JSON.
4. Si una línea no tiene `ItemCode`, se ignora para el flujo operativo.
5. Si la línea está abierta pero SAP no devuelve cantidad abierta, SmartWMS usa `Quantity` como fallback inicial.

## Impacto en el flujo End-to-End

La fase 10.11.3 ya consumía `ISapSalesOrderReader`. Con esta fase, el orquestador puede iniciar el flujo usando una orden real de SAP:

```text
SAP Sales Order
↓
Create Picking
↓
Picking Execution
↓
Packing
↓
Shipping
↓
Delivery SAP
```

## Endpoint disponible

El endpoint ya existente queda ahora conectado a lectura real:

```text
GET /api/sap/sales-orders/{docEntry}
```

## Configuración requerida

En `appsettings.json`:

```json
"SAP": {
  "Mode": "ServiceLayer",
  "CompanyDb": "SBODEMO",
  "ServiceLayerBaseUrl": "https://servidor-sap:50000/b1s/v1",
  "UserName": "manager",
  "Password": "password"
}
```

> La sesión de Service Layer debe iniciarse antes usando:
>
> `POST /api/sap/service-layer/login`

## Siguiente fase sugerida

Fase 10.11.5 — Picking/Packing/Shipping Integration Real.
