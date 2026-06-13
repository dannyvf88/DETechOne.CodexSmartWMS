# Fase 10.5 — Movement Engine

Esta fase agrega la base funcional para movimientos internos de SmartWMS 2.0.

## Incluye

- Entidades de dominio para documento y líneas de movimiento.
- Estados: Open, Confirmed, Cancelled.
- Tipos: Transfer, Replenishment, Relocation, PickingMove, PackingMove, ShippingMove.
- Contratos DTO/Request.
- Servicio de aplicación `IMovementService`.
- Repositorio inicial `InMemoryMovementRepository`.
- API REST protegida por JWT.
- Script SQL Server draft para persistencia física futura.

## Endpoints

```text
GET  /api/movement/open
GET  /api/movement/{movementId}
POST /api/movement
POST /api/movement/confirm
POST /api/movement/cancel
```

## Nota técnica

El repositorio actual es in-memory para estabilizar contratos y flujo de negocio antes de conectar definitivamente SQL Server/HANA/SAP B1.
