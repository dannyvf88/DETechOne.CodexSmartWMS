# Fase 10.7 — Packing Engine

## Objetivo

Agregar la primera base productiva del motor de empaque de SmartWMS 2.0, manteniendo el patrón de arquitectura definido en las fases anteriores.

## Alcance incluido

- Entidades de dominio para packing.
- Estados de documento y línea.
- Contratos DTO y requests.
- Servicio de aplicación `IPackingService`.
- Repositorio `IPackingRepository`.
- Implementación temporal en memoria.
- Controlador REST protegido por JWT.
- Script SQL Server de referencia.

## Endpoints agregados

```text
GET  /api/packing/open
GET  /api/packing/{packingId}
POST /api/packing
POST /api/packing/pack
POST /api/packing/close
POST /api/packing/cancel
```

## Reglas principales

- Un packing nace a partir de un picking terminado o liberado para empaque.
- No se permite empacar cantidades mayores a la cantidad pickeada.
- La primera lectura de empaque cambia el documento de `Open` a `InProgress`.
- Cuando todas las líneas están completas, el documento se cierra automáticamente.
- Un packing completado no puede cancelarse.
- Un packing cancelado no puede completarse ni empacarse.

## Siguiente fase sugerida

Fase 10.8 — Shipping Engine.
