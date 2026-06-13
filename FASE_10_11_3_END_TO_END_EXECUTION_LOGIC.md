# Fase 10.11.3 — End-to-End Flow Execution Logic

Esta fase agrega lógica ejecutable al flujo **Order to Delivery**.

## Objetivo

Convertir el orquestador de la fase 10.11.2 en un flujo progresivo capaz de ejecutar las etapas operativas:

1. SAP Sales Order leído desde SAP.
2. Picking creado.
3. Picking ejecutado/cerrado.
4. Packing creado.
5. Packing ejecutado/cerrado.
6. Shipping creado.
7. Shipping confirmado.
8. Delivery SAP creado o saltado según solicitud.

## Endpoint agregado

```http
POST /api/end-to-end/order-to-delivery/execute
```

## Request

```json
{
  "flowId": "00000000-0000-0000-0000-000000000000",
  "autoCompletePicking": true,
  "autoCompletePacking": true,
  "autoConfirmShipping": true,
  "createSapDelivery": false,
  "packageCode": "PKG-001"
}
```

## Comportamiento

- Si `autoCompletePicking` es `false` y hay cantidades pendientes, el flujo queda en `WaitingForOperator`.
- Si `autoCompletePacking` es `false` y hay cantidades pendientes, el flujo queda en `WaitingForOperator`.
- Si `autoConfirmShipping` es `false`, el flujo queda esperando confirmación manual de shipping.
- Si `createSapDelivery` es `false`, la creación de delivery SAP se marca como `Skipped` y el flujo queda `Completed`.
- Si `createSapDelivery` es `true`, se invoca `IShippingService.CreateSapDeliveryAsync`.

## Archivos principales

- `Contracts/Requests/EndToEnd/ExecuteOrderToDeliveryFlowRequest.cs`
- `Application/EndToEnd/EndToEndFlowStepCode.cs`
- `Application/EndToEnd/EndToEndFlowStepStatus.cs`
- `Application/EndToEnd/EndToEndFlowStatus.cs`
- `Application/EndToEnd/IEndToEndFlowOrchestrator.cs`
- `Application/EndToEnd/OrderToDeliveryFlowOrchestrator.cs`
- `API/Controllers/EndToEndController.cs`

## Nota técnica

Esta implementación sigue siendo compatible con los servicios actuales en memoria/repositorios base. La fase posterior debe reemplazar gradualmente piezas mock/skeleton por persistencia real y lectura real de SAP.
