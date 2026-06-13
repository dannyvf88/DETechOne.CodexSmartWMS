# Fase 10.6 — Picking Engine

## Objetivo

Agregar el motor base de picking para SmartWMS 2.0, preparado para operar con documentos de origen SAP Business One, escaneo desde handheld y cierre controlado antes de packing/shipping.

## Componentes agregados

### Domain

- `PickingDocument`
- `PickingLine`
- `PickingStatus`
- `PickingLineStatus`

### Contracts

- `PickingDocumentDto`
- `PickingLineDto`
- `CreatePickingRequest`
- `CreatePickingLineRequest`
- `ScanPickingItemRequest`
- `ClosePickingRequest`
- `CancelPickingRequest`

### Application

- `IPickingService`
- `PickingService`
- `IPickingRepository`
- `PickingMapper`

### Infrastructure

- `InMemoryPickingRepository`
- `PickingEngine.SqlServer.sql`

### API

- `PickingController`

## Endpoints iniciales

```http
GET  /api/picking/open
GET  /api/picking/{pickingId}
POST /api/picking
POST /api/picking/scan
POST /api/picking/close
POST /api/picking/cancel
```

## Reglas funcionales iniciales

- Un picking inicia en estado `Open`.
- Al primer escaneo cambia a `InProgress`.
- No se permite escanear más cantidad que la requerida.
- Una línea puede quedar `Pending`, `Partial`, `Completed` o `Cancelled`.
- El documento se cierra automáticamente si todas las líneas quedan completas.
- También puede cerrarse manualmente si todas las líneas están completas.
- Un picking completado no puede cancelarse.
- Un picking cancelado no puede escanearse ni cerrarse.

## Nota técnica

La implementación actual usa repositorio en memoria para validar arquitectura y contratos. La persistencia productiva deberá migrarse a SQL/HANA/UDT según la estrategia de Metadata Installer.
