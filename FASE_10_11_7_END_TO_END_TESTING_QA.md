# Fase 10.11.7 — End-to-End Testing & QA

## Objetivo

Validar el flujo operativo completo **SAP Sales Order → Picking → Packing → Shipping → SAP Delivery** antes de continuar con Device API, Audit, Alerts y Dashboard Metrics.

Esta fase agrega una primera batería de pruebas automatizadas del orquestador End-to-End usando servicios fake en memoria para aislar reglas de negocio sin depender de SAP Business One, Service Layer, DI API, SQL Server o HANA durante la compilación.

## Alcance implementado

### Pruebas agregadas

Archivo:

```text
DETechOne.SmartWMS.Tests/EndToEnd/OrderToDeliveryFlowOrchestratorTests.cs
```

Casos cubiertos:

1. `StartOrderToDeliveryFlowAsync_WhenSalesOrderHasOpenLines_CreatesPickingAndInitialFlow`
   - Lee una orden SAP simulada.
   - Detecta líneas abiertas.
   - Crea Picking.
   - Inicializa pasos del flujo.

2. `StartOrderToDeliveryFlowAsync_WhenSalesOrderHasNoOpenLines_ReturnsValidationFailure`
   - Valida que una orden sin cantidades abiertas no cree Picking.
   - Retorna `E2E_NO_OPEN_LINES`.

3. `ExecuteAsync_WhenAllAutomationFlagsEnabled_CompletesDeliveryCreation`
   - Ejecuta Picking automático.
   - Crea y cierra Packing.
   - Crea y confirma Shipping.
   - Crea entrega SAP simulada.
   - Termina en estado `DeliveryCreated`.

4. `ExecuteAsync_WhenPickingIsPendingAndAutoCompleteIsDisabled_WaitsForOperator`
   - Valida que el flujo espere al operador cuando el picking tiene pendientes y no se habilita automatización.
   - Termina en estado `WaitingForOperator`.

## Criterios de aceptación

- La solución compila en Visual Studio 2022.
- `DETechOne.SmartWMS.Tests` ejecuta correctamente las pruebas existentes y las nuevas pruebas End-to-End.
- El orquestador puede iniciar un flujo desde una orden SAP con líneas abiertas.
- El orquestador no permite avanzar con orden sin líneas abiertas.
- El flujo automatizado puede llegar hasta `DeliveryCreated` usando servicios fake.
- El flujo puede quedar en espera de operador si no se permite completar Picking de forma automática.

## Cómo ejecutar

Desde Visual Studio 2022:

```text
Test > Run All Tests
```

Desde consola, si está disponible el SDK:

```bash
dotnet test DETechOne.SmartWMS.sln
```

## Pendientes para QA productivo

La fase actual valida reglas del orquestador con fakes. Para QA productivo todavía faltan pruebas integradas contra ambiente real o sandbox:

- Service Layer real.
- SAP DI API real.
- Base SQL Server/HANA real.
- UDT/UDO instalados.
- Ordenes SAP reales con lotes/series.
- Escaneo real desde Zebra TC15.
- Creación real de entrega SAP.

## Resultado

Con esta fase, el backend cuenta con una primera red de seguridad automatizada para el flujo End-to-End y queda preparado para continuar con:

```text
Fase 10.12 — Device / Zebra TC15 API
```
