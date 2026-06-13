# Fase 10.13 — Audit & Alert Engine

## Objetivo

Agregar la base del motor de auditoría y alertas operativas de SmartWMS 2.0.

Esta fase permite registrar eventos críticos del WMS y consultar alertas operativas generadas por procesos como Picking, Packing, Shipping, integración SAP, dispositivos Zebra TC15 y flujo End-to-End.

## Componentes incluidos

### Domain

- `AuditLogEntry`
- `OperationalAlert`
- `AlertSeverity`
- `AlertStatus`

### Contracts

- `AuditLogEntryDto`
- `OperationalAlertDto`
- `CreateAuditLogRequest`
- `AuditLogQueryRequest`
- `CreateOperationalAlertRequest`
- `AlertQueryRequest`

### Application

- `IAuditService`
- `AuditService`
- `IAuditLogRepository`
- `IAlertService`
- `AlertService`
- `IAlertRepository`

### Infrastructure

- `InMemoryAuditLogRepository`
- `InMemoryAlertRepository`

### API

- `AuditController`
- `AlertsController`

## Endpoints agregados

```text
POST /api/audit/events
GET  /api/audit/events

POST /api/alerts
GET  /api/alerts
POST /api/alerts/{alertId}/acknowledge
POST /api/alerts/{alertId}/resolve
```

## Consideraciones

La implementación inicial usa repositorios en memoria para conservar compatibilidad con el estado actual del backend y permitir pruebas rápidas sin depender todavía de tablas físicas.

En la siguiente etapa productiva, estos repositorios deberán persistir en UDTs/Tablas SmartWMS:

```text
@DTO_SW_AUDIT
@DTO_SW_ALERT
```

## Uso esperado

El Audit Engine deberá ser invocado desde procesos como:

- Login
- Registro de dispositivos
- Inicio/cierre de Picking
- Inicio/cierre de Packing
- Confirmación de Shipping
- Creación de Delivery SAP
- Errores de Service Layer / DI API
- Eventos del flujo End-to-End

El Alert Engine deberá generar alertas para:

- Error SAP
- Inventario insuficiente
- Picking detenido
- Packing inconsistente
- Dispositivo inactivo
- Sesión de scanner cancelada
- Delivery no creado
```
