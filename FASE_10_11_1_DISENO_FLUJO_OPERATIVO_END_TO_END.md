# Fase 10.11.1 — Diseño del Flujo Operativo End-to-End

## Objetivo

Definir el flujo operativo completo de SmartWMS 2.0 desde una Orden de Venta en SAP Business One hasta la creación de la Entrega en SAP, conectando los motores ya construidos:

- SAP Integration Foundation
- Inventory Engine
- Movement Engine
- Picking Engine
- Packing Engine
- Shipping Engine
- Database Schema Installer Real

Esta fase es de diseño técnico-funcional. No modifica lógica compilable. Sirve como blueprint para implementar la Fase 10.11.2.

---

## Alcance del flujo

```text
SAP Sales Order
    ↓
Validación de orden
    ↓
Disponibilidad de inventario
    ↓
Creación de Picking
    ↓
Ejecución de Picking con escaneo
    ↓
Cierre de Picking
    ↓
Creación de Packing
    ↓
Ejecución de Packing
    ↓
Cierre de Packing
    ↓
Creación de Shipping
    ↓
Confirmación de Shipping
    ↓
Creación de Delivery SAP
    ↓
Cierre operacional del ciclo
```

---

## Principio de diseño

SmartWMS 2.0 debe comportarse como un orquestador operativo, no como un duplicador de SAP.

SAP Business One conserva la verdad documental:

- Orden de venta
- Socio de negocio
- Artículos
- Almacenes
- Lotes/series
- Entrega final

SmartWMS conserva la verdad operativa:

- Tareas de picking
- Escaneos
- Ubicaciones
- Empaque
- Shipping
- Auditoría
- Alertas
- Dispositivos
- Productividad

---

## Agregados principales

### SalesOrderWorkFlow

Representa el flujo de trabajo operativo asociado a una orden SAP.

Responsabilidades:

- Validar si la orden puede ser procesada.
- Relacionar Picking, Packing y Shipping.
- Evitar duplicidad de flujos.
- Controlar el estado end-to-end.

Campos sugeridos:

```text
WorkflowId
SapDocEntry
SapDocNum
CardCode
CardName
WarehouseCode
Status
CreatedAt
CreatedBy
CompletedAt
CompletedBy
DeliveryDocEntry
DeliveryDocNum
```

---

### Picking

Responsable de surtir físicamente los artículos.

Estados:

```text
OP  = Open
IP  = InProgress
TE  = Terminated
CA  = Cancelled
ER  = Error
```

Reglas:

- No se puede crear Picking si la orden SAP no existe.
- No se puede crear Picking si la orden ya tiene un flujo activo.
- No se puede cerrar Picking si existen líneas pendientes.
- No se puede cancelar Picking si ya generó Packing cerrado.

---

### Packing

Responsable de empacar lo surtido.

Estados:

```text
OP  = Open
IP  = InProgress
TE  = Terminated
CA  = Cancelled
ER  = Error
```

Reglas:

- No se puede crear Packing si Picking no está terminado.
- No se puede empacar más cantidad de la surtida.
- No se puede cerrar Packing con líneas pendientes.
- No se puede cancelar Packing si ya generó Shipping confirmado.

---

### Shipping

Responsable de preparar la salida y crear la entrega SAP.

Estados:

```text
OP  = Open
CF  = Confirmed
DL  = DeliveryCreated
CA  = Cancelled
ER  = Error
```

Reglas:

- No se puede crear Shipping si Packing no está terminado.
- No se puede crear Delivery SAP si Shipping no está confirmado.
- No se puede crear Delivery SAP dos veces para el mismo Shipping.
- Si SAP falla, Shipping queda en estado ER con detalle del error.

---

## Transiciones end-to-end

| Paso | Origen | Acción | Destino |
|---|---|---|---|
| 1 | SAP Sales Order | StartOrderFlow | Workflow OP |
| 2 | Workflow OP | CreatePicking | Picking OP |
| 3 | Picking OP | ScanItem | Picking IP |
| 4 | Picking IP | ClosePicking | Picking TE |
| 5 | Picking TE | CreatePacking | Packing OP |
| 6 | Packing OP | PackItem | Packing IP |
| 7 | Packing IP | ClosePacking | Packing TE |
| 8 | Packing TE | CreateShipping | Shipping OP |
| 9 | Shipping OP | ConfirmShipping | Shipping CF |
| 10 | Shipping CF | CreateSapDelivery | Shipping DL |
| 11 | Shipping DL | CompleteWorkflow | Workflow TE |

---

## Validaciones críticas

### Validación SAP Sales Order

Antes de iniciar el flujo:

```text
- Existe DocEntry.
- Documento no está cancelado.
- Documento no está cerrado.
- Tiene líneas abiertas.
- Tiene almacén válido.
- Tiene artículos inventariables.
```

### Validación de inventario

Antes de crear Picking:

```text
- Existencia disponible.
- Ubicación válida.
- Lote/serie disponible si aplica.
- No reservado por otro flujo.
```

### Validación de Picking

Antes de cerrar Picking:

```text
Cantidad escaneada >= cantidad requerida
No hay líneas en error
No hay artículos pendientes obligatorios
Usuario tiene permiso de cierre
```

### Validación de Packing

Antes de cerrar Packing:

```text
Cantidad empacada == cantidad pickeada
Unidad logística asignada
Caja/contenedor válido si aplica
No hay líneas pendientes
```

### Validación de Shipping

Antes de crear Delivery:

```text
Shipping confirmado
Packing terminado
No existe Delivery previa
SAP disponible
Usuario autorizado
```

---

## Eventos de dominio sugeridos

```text
OrderWorkflowStarted
PickingCreated
PickingItemScanned
PickingClosed
PackingCreated
PackingItemPacked
PackingClosed
ShippingCreated
ShippingConfirmed
SapDeliveryCreated
OrderWorkflowCompleted
OrderWorkflowFailed
```

Estos eventos serán usados después para:

- Auditoría
- Alertas
- Métricas
- Integración con dashboard
- Notificaciones operativas

---

## Endpoints propuestos para 10.11.2

### Order Flow Controller

```text
POST /api/order-flow/start
GET  /api/order-flow/{workflowId}
GET  /api/order-flow/by-sales-order/{docEntry}
POST /api/order-flow/{workflowId}/create-picking
POST /api/order-flow/{workflowId}/close-picking
POST /api/order-flow/{workflowId}/create-packing
POST /api/order-flow/{workflowId}/close-packing
POST /api/order-flow/{workflowId}/create-shipping
POST /api/order-flow/{workflowId}/confirm-shipping
POST /api/order-flow/{workflowId}/create-delivery
POST /api/order-flow/{workflowId}/complete
POST /api/order-flow/{workflowId}/cancel
```

### Endpoint recomendado de orquestación completa para pruebas

```text
POST /api/order-flow/run-demo
```

Uso:

- Solo ambiente Development/Staging.
- Ejecuta el flujo con servicios mock.
- Permite probar la cadena completa sin SAP real.

---

## Contratos sugeridos

### StartOrderFlowRequest

```text
SapDocEntry
WarehouseCode
RequestedBy
DeviceCode
```

### OrderFlowDto

```text
WorkflowId
SapDocEntry
SapDocNum
CardCode
CardName
Status
PickingId
PackingId
ShippingId
DeliveryDocEntry
DeliveryDocNum
CreatedAt
CompletedAt
```

### OrderFlowStepResultDto

```text
WorkflowId
Step
Success
Message
ReferenceId
SapDocEntry
SapDocNum
```

---

## Servicios de aplicación sugeridos

```text
IOrderFlowService
ISalesOrderValidationService
IOrderFlowStateService
IOrderFlowOrchestrator
IOrderFlowRepository
```

Responsabilidad principal:

### IOrderFlowOrchestrator

Coordina el flujo sin ejecutar detalles internos de cada engine.

```text
StartAsync
CreatePickingAsync
ClosePickingAsync
CreatePackingAsync
ClosePackingAsync
CreateShippingAsync
ConfirmShippingAsync
CreateDeliveryAsync
CompleteAsync
CancelAsync
```

---

## Integración con motores existentes

### Inventory Engine

Usado para:

```text
- Validar disponibilidad
- Reservar inventario
- Liberar reserva si se cancela
```

### Picking Engine

Usado para:

```text
- Crear tarea de picking
- Registrar escaneos
- Cerrar picking
```

### Packing Engine

Usado para:

```text
- Crear packing desde picking cerrado
- Registrar empaque
- Cerrar packing
```

### Shipping Engine

Usado para:

```text
- Crear shipping desde packing cerrado
- Confirmar salida
- Solicitar Delivery SAP
```

### SAP Integration Foundation

Usado para:

```text
- Leer Sales Order
- Leer Business Partner
- Leer Item Master
- Crear Delivery
- Crear Inventory Transfer si aplica
```

---

## Tablas UDT/UDO sugeridas

Basado en la evolución natural del SmartWMS actual:

```text
@DTO_SW_OJ_HFLOW       Header flujo E2E
@DTO_SW_OJ_LFLOW       Detalle de pasos del flujo
@DTO_SW_OJ_HPICKING    Header picking
@DTO_SW_OJ_LPICKING    Líneas picking
@DTO_SW_OJ_HPACKING    Header packing
@DTO_SW_OJ_LPACKING    Líneas packing
@DTO_SW_OJ_HSHIPPING   Header shipping
@DTO_SW_OJ_LSHIPPING   Líneas shipping
@DTO_SW_LOG            Log técnico
@DTO_SW_AUDIT          Auditoría funcional
```

---

## Reglas anti-duplicidad

```text
Una Sales Order abierta solo puede tener un Workflow activo.
Un Picking cerrado solo puede generar un Packing activo.
Un Packing cerrado solo puede generar un Shipping activo.
Un Shipping confirmado solo puede crear una Delivery SAP.
Una Delivery SAP creada no puede repetirse aunque el endpoint se invoque dos veces.
```

---

## Manejo de errores

### Error operativo

Ejemplo:

```text
Inventario insuficiente
Artículo no escaneado
Cantidad excedida
Ubicación inválida
```

Respuesta:

```text
HTTP 400
ApiResponse.Fail
```

### Error SAP

Ejemplo:

```text
DI API error
Service Layer timeout
Documento cerrado
Lote no disponible
```

Respuesta:

```text
HTTP 502 o HTTP 400 según el caso
Shipping queda en ER
Se registra SapErrorCode y SapErrorMessage
```

### Error técnico

Ejemplo:

```text
Excepción no controlada
Base de datos no disponible
Configuración faltante
```

Respuesta:

```text
HTTP 500
Global Exception Middleware
Log técnico
```

---

## Criterios de aceptación de la fase 10.11.2

La implementación debe cumplir:

```text
- Compila sin warnings tratados como errores.
- Tiene contratos StartOrderFlowRequest, OrderFlowDto y OrderFlowStepResultDto.
- Tiene servicio IOrderFlowOrchestrator.
- Tiene controlador OrderFlowController.
- Permite iniciar flujo desde DocEntry SAP.
- Evita flujo duplicado por orden.
- Orquesta Picking → Packing → Shipping → Delivery.
- Usa interfaces existentes, no acopla controllers a infraestructura.
- Puede funcionar con mocks/stubs sin SAP real.
- Deja preparada la conexión con SAP real para 10.11.3.
```

---

## Próxima fase recomendada

```text
Fase 10.11.2 — End-to-End Flow Contracts & Orchestrator Skeleton
```

En esta fase se debe agregar código compilable:

```text
Domain/Entities/OrderFlow
Domain/Enums/OrderFlowStatus
Contracts/Requests/OrderFlow
Contracts/Dtos/OrderFlow
Application/OrderFlow/IOrderFlowOrchestrator
Application/OrderFlow/IOrderFlowService
Infrastructure/OrderFlow/InMemoryOrderFlowRepository o RepositoryBase
API/Controllers/OrderFlowController
```

