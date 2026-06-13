# Fase 10.4 — Inventory Engine

Esta fase agrega el primer motor funcional de inventario para SmartWMS 2.0.

## Componentes agregados

- Entidades de dominio:
  - InventoryBalance
  - InventoryReservation
  - InventoryTransaction

- Enums:
  - InventoryTransactionType
  - InventoryReservationStatus
  - StockAvailabilityStatus

- Contratos:
  - InventoryBalanceDto
  - InventoryAvailabilityDto
  - InventoryReservationDto
  - InventoryTransactionDto
  - GetInventoryAvailabilityRequest
  - AdjustInventoryRequest
  - ReserveInventoryRequest
  - ReleaseInventoryReservationRequest

- Application:
  - IInventoryService
  - InventoryService
  - IInventoryRepository

- Infrastructure:
  - InMemoryInventoryRepository
  - Script de referencia SQL Server

- API:
  - GET /api/inventory/availability
  - POST /api/inventory/adjustments
  - POST /api/inventory/reservations
  - POST /api/inventory/reservations/release

## Nota productiva

El repositorio agregado es en memoria para permitir validar el flujo funcional sin requerir todavía tablas físicas. La siguiente evolución debe conectar `IInventoryRepository` a UDT/SQL/HANA según el Metadata Installer productivo.
