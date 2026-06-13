# Fase 10.11.6 — Delivery Creation Real

## Objetivo

Conectar el proceso de Shipping con la creación real de entregas en SAP Business One usando Service Layer como adaptador productivo inicial.

## Alcance implementado

- Implementación real de `ISapDeliveryService` mediante `SapDeliveryService`.
- Creación de Delivery Notes en SAP mediante `POST DeliveryNotes`.
- Validación de estado de Shipping antes de crear entrega.
- Mapeo de líneas WMS hacia `DocumentLines`.
- Soporte inicial de lotes mediante `BatchNumbers`.
- Lectura de respuesta Service Layer para obtener `DocEntry` y `DocNum`.
- Registro de UDFs base:
  - `U_DTO_SW_ShippingId`
  - `U_DTO_SW_ShippingNo`

## Endpoint impactado

```http
POST /api/shipping/create-delivery
```

También es usado desde el flujo End-to-End cuando `CreateSapDelivery = true`:

```http
POST /api/end-to-end/order-to-delivery/execute
```

## Configuración requerida

En `appsettings.json`:

```json
{
  "SAP": {
    "Mode": "ServiceLayer",
    "ServiceLayerBaseUrl": "https://servidor-sap:50000/b1s/v1"
  }
}
```

## Nota técnica

La implementación actual crea una entrega directa con `ItemCode`, `Quantity`, `WarehouseCode` y lotes cuando aplique.

Cuando la persistencia del WMS almacene el vínculo exacto contra la orden de venta SAP, la siguiente mejora será activar líneas baseadas en pedido:

- `BaseType = 17`
- `BaseEntry = SalesOrderDocEntry`
- `BaseLine = SalesOrderLineNum`

## Siguiente fase sugerida

**Fase 10.11.7 — End-to-End Testing & QA**
