# Fase 10.11.2 — End-to-End Flow Contracts & Orchestrator Skeleton

## Objetivo

Crear la primera base compilable del flujo operativo completo:

```text
SAP Sales Order
    ↓
Picking
    ↓
Packing
    ↓
Shipping
    ↓
SAP Delivery
```

Esta fase no ejecuta todavía el ciclo completo automático. Su responsabilidad es dejar el contrato, el orquestador y el primer endpoint para iniciar el flujo desde una orden de venta de SAP y crear el documento inicial de picking.

## Componentes agregados

### Contracts

- `StartOrderToDeliveryFlowRequest`
- `OrderToDeliveryFlowDto`
- `EndToEndFlowStepDto`

### Application

- `IEndToEndFlowOrchestrator`
- `OrderToDeliveryFlowOrchestrator`
- `IEndToEndFlowStateStore`
- `InMemoryEndToEndFlowStateStore`
- `OrderToDeliveryFlowState`
- `OrderToDeliveryFlowMapper`
- `EndToEndFlowStatus`

### API

- `EndToEndController`

## Endpoints

```text
POST /api/end-to-end/order-to-delivery/start
GET  /api/end-to-end/order-to-delivery/{flowId}
```

## Flujo implementado en esta fase

1. Recibe `SalesOrderDocEntry`.
2. Lee la orden de venta usando `ISapSalesOrderReader`.
3. Valida líneas abiertas.
4. Resuelve almacén.
5. Crea un `Picking` usando `IPickingService`.
6. Registra el estado del flujo en `IEndToEndFlowStateStore`.
7. Devuelve el `FlowId`, `PickingId`, `PickingNumber` y pasos pendientes.

## Siguiente fase sugerida

### Fase 10.11.3 — End-to-End Flow Transition Handlers

Implementar transiciones reales:

```text
PickingCompleted → PackingCreated
PackingCompleted → ShippingCreated
ShippingConfirmed → SapDeliveryCreated
```

Esto permitirá que el orquestador reaccione al avance de cada engine.
