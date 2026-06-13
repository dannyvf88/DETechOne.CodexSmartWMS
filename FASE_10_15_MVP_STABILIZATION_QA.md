# Fase 10.15 — MVP Stabilization & QA

## Objetivo

Cerrar el backend MVP de SmartWMS 2.0 con una capa mínima de estabilización, validación operativa y checklist de liberación antes de iniciar la interfaz de usuario.

Esta fase no cambia la lógica funcional principal. Su propósito es ayudar a validar que los componentes críticos del backend estén registrados, disponibles y listos para pruebas integrales.

## Componentes agregados

### API

```text
GET /api/readiness/mvp
```

Endpoint anónimo de readiness para validar el estado general del backend MVP.

### Application

```text
IMvpReadinessService
MvpReadinessService
```

Valida resolución de dependencias críticas:

```text
Core clock
Inventory Engine
Movement Engine
Picking Engine
Packing Engine
Shipping Engine
End-to-End Orchestrator
SAP Connection Manager
Database Schema Installer
Audit Engine
Alert Engine
Dashboard Metrics
Database Connection
```

### Contracts

```text
MvpReadinessDto
MvpReadinessCheckDto
```

DTOs de salida para revisión de readiness.

## Estados posibles

```text
READY
READY_WITH_WARNINGS
NOT_READY
```

### READY

Todos los checks críticos están correctos.

### READY_WITH_WARNINGS

El backend cargó sus servicios principales, pero algún componente externo no está disponible o no está configurado, por ejemplo la conexión a base de datos.

### NOT_READY

Falta alguna dependencia crítica de backend.

## Checklist MVP Backend

Antes de iniciar la UI, validar:

```text
[ ] Compila en Debug
[ ] Compila en Release
[ ] Restore NuGet limpio
[ ] Run All Tests exitoso
[ ] Swagger abre correctamente
[ ] GET /api/health responde OK
[ ] GET /api/health/database responde OK o WARNING esperado por ambiente
[ ] GET /api/readiness/mvp responde READY o READY_WITH_WARNINGS controlado
[ ] Login JWT funciona con usuario temporal
[ ] Endpoints Picking disponibles
[ ] Endpoints Packing disponibles
[ ] Endpoints Shipping disponibles
[ ] Endpoints SAP disponibles
[ ] Endpoints Device disponibles
[ ] Endpoints Audit/Alert disponibles
[ ] Endpoint Dashboard disponible
```

## Smoke Test recomendado

1. Ejecutar API.
2. Abrir Swagger.
3. Probar:

```text
GET /api/health
GET /api/readiness/mvp
POST /api/auth/login
GET /api/dashboard/overview
GET /api/sap/status
```

4. Ejecutar pruebas unitarias.
5. Revisar warnings tratados como errores.

## Resultado

Con esta fase, el backend MVP queda cerrado a nivel de arquitectura base y listo para iniciar:

```text
Fase 11.0 — SmartWMS UI Foundation
```
