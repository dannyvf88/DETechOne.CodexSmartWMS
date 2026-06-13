# Fase 10.14 — Dashboard Metrics API

## Objetivo

Agregar una capa inicial de métricas operativas para alimentar el futuro dashboard de SmartWMS 2.0.

## Alcance incluido

- Contratos de dashboard en `DETechOne.SmartWMS.Contracts`.
- Servicio de métricas en `DETechOne.SmartWMS.Application`.
- Integración con repositorios actuales de Picking, Packing, Shipping y Alertas.
- Adaptador API para consultar métricas de dispositivos Zebra/handheld registrados en memoria.
- Endpoint REST para obtener un resumen operativo consolidado.

## Endpoint agregado

```text
GET /api/dashboard/overview
```

Parámetros opcionales:

```text
fromUtc

toUtc
warehouseCode
```

Ejemplo:

```text
GET /api/dashboard/overview?warehouseCode=01
```

## Métricas expuestas

- Picking abiertos, en progreso, completados y cancelados.
- Cantidad requerida, procesada y pendiente de picking.
- Packing abiertos, en progreso, completados y cancelados.
- Cantidad requerida, procesada y pendiente de packing.
- Shipping abiertos, confirmados, con delivery creado y cancelados.
- Dispositivos online.
- Alertas abiertas, reconocidas, resueltas, críticas, error y warning.
- Contadores agregados listos para tarjetas KPI.

## Nota técnica

Las métricas actuales se apoyan en los repositorios en memoria existentes. Cuando se implemente persistencia real contra SQL Server/HANA, este servicio podrá evolucionar a consultas optimizadas por agregación.
