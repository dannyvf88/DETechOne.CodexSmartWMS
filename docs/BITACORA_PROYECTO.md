# Bitacora del Proyecto SmartWMS

Fecha de corte: 2026-06-13
Repositorio local: `C:\Datos\Repositorios\SmrtWMS_20\Codex\DETechOne.CodexSmartWMS`
Rama actual: `codex/phase-11-operational-pages`

## Estado general

El proyecto ya fue importado a Git, tiene rama `main` sincronizada con remoto y se esta trabajando la Fase 11 sobre UI operacional. La rama activa contiene la base de autenticacion estabilizada y varias pantallas operativas conectadas a endpoints reales de la API.

El working tree estaba limpio al iniciar esta bitacora. La unica modificacion nueva de este corte es este documento.

## Avance registrado

### Fase 11 - Corte 2026-06-13 posterior

- Devices UI operacional
  - Pantalla `/devices` reemplazada desde placeholder a operacion funcional.
  - Registro de dispositivos contra `api/devices/register`.
  - Consulta de dispositivos online contra `api/devices/online`.
  - Consulta de detalle contra `api/devices/{deviceCode}`.
  - Heartbeat contra `api/devices/heartbeat`.
  - Inicio, scan, completado y cancelacion de sesiones scanner contra endpoints `api/devices/scanner-sessions/*`.

- Movement UI operacional
  - Nueva pantalla `/movement`.
  - Entrada agregada al menu lateral.
  - Listado de movimientos abiertos contra `api/movement/open`.
  - Creacion, seleccion, confirmacion y cancelacion de movimientos internos contra API.

- Audit/Alerts UI operacional
  - Nueva pantalla `/audit`.
  - Nueva pantalla `/alerts`.
  - Entradas agregadas al menu lateral.
  - Consulta y registro de eventos contra `api/audit/events`.
  - Consulta, creacion, reconocimiento y resolucion de alertas contra `api/alerts`.

- E2E Operations UI operacional
  - Nueva pantalla `/end-to-end`.
  - Entrada agregada al menu lateral.
  - Inicio de flujo order-to-delivery contra `api/end-to-end/order-to-delivery/start`.
  - Ejecucion de flujo contra `api/end-to-end/order-to-delivery/execute`.
  - Consulta de flujo por `FlowId` contra `api/end-to-end/order-to-delivery/{flowId}`.
  - Visualizacion de resumen, documentos generados y etapas del flujo.

- SAP Operations UI operacional
  - Nueva pantalla `/sap-operations`.
  - Entrada agregada al menu lateral.
  - Consulta de status SAP contra `api/sap/status`.
  - Login/logout Service Layer contra `api/sap/service-layer/*`.
  - Consulta de Business Partner, Sales Order, Item y Warehouse contra endpoints SAP existentes.
  - Creacion de inventory transfer contra `api/sap/inventory-transfers`.

### Base del repositorio

- `0629cf8 Initial SmartWMS project import`
  - Importacion inicial del proyecto SmartWMS.
  - Solucion .NET organizada por capas: API, Application, Contracts, Domain, Infrastructure, SAP, Installer, Tasks, Tests y Web.

### Fase 11 - UI foundation y autenticacion

- `f9f8976 Stabilize authentication UI routes`
  - Estabilizacion de rutas y flujo de autenticacion en la UI.

- `db5ee41 Handle API error responses in web client`
  - Manejo de respuestas de error de API desde el cliente web.

- `1017bdf Document authentication UI E2E validation`
  - Documentacion de validacion E2E de autenticacion.
  - Este cambio ya esta integrado en `main`.

### Fase 11 - Pantallas operacionales

Rama: `codex/phase-11-operational-pages`

- `7e31f8c Connect dashboard to operational metrics API`
  - Dashboard conectado a `api/dashboard/overview`.
  - Uso de DTOs desde `DETechOne.SmartWMS.Contracts`.
  - Ajustes visuales en estilos compartidos.

- `9f84d4c Add operational inventory page`
  - Pantalla Inventory con consulta de disponibilidad.
  - Formulario para ajustes de inventario contra `api/inventory/adjustments`.

- `8b244a1 Add operational picking page`
  - Pantalla Picking con listado de documentos abiertos.
  - Creacion, seleccion, escaneo, cierre y cancelacion de picking contra API.

- `5c04154 Add operational packing page`
  - Pantalla Packing con listado de documentos abiertos.
  - Creacion, seleccion, empaque de lineas, cierre y cancelacion de packing contra API.

- `cffbbab Add operational shipping page`
  - Pantalla Shipping con listado de documentos abiertos.
  - Creacion, seleccion, confirmacion, creacion de delivery SAP y cancelacion de shipping contra API.

## Validaciones ejecutadas

Ultima validacion completa registrada:

- QA final Fase 11 - 2026-06-13
  - `dotnet build DETechOne.SmartWMS.sln`: correcto, 0 errores.
  - `dotnet test DETechOne.SmartWMS.sln --no-build`: correcto, 13 pruebas aprobadas.
  - Smoke Web: `/dashboard`, `/inventory`, `/movement`, `/picking`, `/packing`, `/shipping`, `/end-to-end`, `/sap-operations`, `/devices`, `/alerts`, `/audit`, `/profile`: HTTP 200.
  - Smoke API: login, dashboard overview, SAP status, device register, audit create, alert create y movement create: correcto.
  - Smoke API E2E: validacion protegida de `start` con `SalesOrderDocEntry = 0`: HTTP 400 esperado.

- Validaciones previas
  - Smoke API Shipping: crear, confirmar y consultar shippings abiertos: correcto.
  - Smoke API Devices/Movement: registrar device, iniciar scanner session, scan, crear y confirmar movement: correcto.
  - Smoke API Audit/Alerts: crear/buscar audit event, crear/reconocer/resolver alerta: correcto.
  - Smoke API SAP: `api/sap/status` con token: correcto, modo `Disabled` en entorno local.

Notas de entorno:

- Para endpoints protegidos de API conviene usar HTTPS. Si se usa HTTP, `UseHttpsRedirection()` puede redirigir y perder el header `Authorization`, provocando falsos 401.
- La validacion visual con navegador integrado no estuvo disponible por error de creacion de proceso del entorno. Se sustituyo por smoke HTTP.

## Punto actual

Estamos al cierre funcional de las pantallas operativas principales de Fase 11:

- Dashboard: conectado.
- Inventory: conectado.
- Movement: conectado.
- Picking: conectado.
- Packing: conectado.
- Shipping: conectado.
- Devices: conectado.
- Alerts: conectado.
- Audit: conectado.
- E2E Operations: conectado.
- SAP Operations: conectado.
- Autenticacion UI: estabilizada y documentada.

La rama `codex/phase-11-operational-pages` esta publicada en remoto y contiene el ultimo avance operativo.

## Pendientes tecnicos

Pendientes inmediatos recomendados:

1. Agregar pruebas automatizadas de UI o componentes para las paginas nuevas si el proyecto adopta bUnit, Playwright u otra herramienta.
2. Validar navegacion completa con usuario real o token real contra API levantada.
3. Revisar UX de errores por endpoint para distinguir validacion de negocio, expiracion de sesion y falla de conectividad.
4. Confirmar si los formularios manuales actuales deben reemplazarse por busquedas reales de documentos origen.
5. Ejecutar una pasada end-to-end completa con SAP Service Layer configurado: Sales Order SAP -> Picking -> Packing -> Shipping -> Delivery SAP.
6. Mantener la rama `codex/phase-11-operational-pages` como respaldo historico de la Fase 11.

## Siguiente paso recomendado

La Fase 11 UI operacional queda lista para merge hacia `main`. Despues del merge, el siguiente bloque recomendado es QA con SAP Service Layer real configurado o definicion de la siguiente fase funcional.

## Convencion para actualizar esta bitacora

Actualizar este archivo al cerrar cada bloque de trabajo con:

- Fecha.
- Rama.
- Commit.
- Alcance entregado.
- Validaciones ejecutadas.
- Pendientes detectados.
- Siguiente paso recomendado.
