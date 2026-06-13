# Fase 10.9 — SAP Integration Foundation

## Objetivo

Establecer la base formal de integración entre SmartWMS 2.0 y SAP Business One sin acoplar todavía la solución a una instalación local del SAP SDK.

## Incluye

- Configuración `SAP` en `appsettings.json`.
- Registro de servicios SAP mediante `AddSmartWmsSap(configuration)`.
- Contratos de aplicación para:
  - Connection Manager.
  - Session Manager.
  - Service Layer Client.
  - Business Partner Reader.
  - Sales Order Reader.
  - Item Master Reader.
  - Warehouse Reader.
  - Inventory Transfer Service.
- DTOs base para documentos y maestros SAP.
- Endpoint de estado SAP.
- Endpoints iniciales de lectura SAP.
- Endpoint para transferencia de inventario SAP.
- Adaptador base para creación de delivery desde Shipping Engine.
- Placeholder seguro para DI API sin dependencia directa a `SAPbobsCOM`.

## Endpoints agregados

```text
GET  /api/sap/status
POST /api/sap/service-layer/login
POST /api/sap/service-layer/logout
GET  /api/sap/business-partners/{cardCode}
GET  /api/sap/sales-orders/{docEntry}
GET  /api/sap/items/{itemCode}
GET  /api/sap/warehouses/{warehouseCode}
POST /api/sap/inventory-transfers
```

## Nota técnica

Esta fase deja la integración preparada en arquitectura limpia. Las clases reales de DI API deben activarse cuando el entorno tenga instalado SAP Business One DI API y las referencias COM correspondientes.

