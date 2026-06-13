# Fase 10.10 — Database Schema Installer Real

## Objetivo

Convertir el instalador de metadata conceptual en un instalador de esquema operativo para SmartWMS 2.0.

## Incluye

- Plan versionado de instalacion `10.10`.
- UDTs operativas para Picking, Packing, Shipping, Movement, Inventory, Devices, Audit y Config.
- UDOs principales para documentos operativos.
- Permisos base de SmartWMS.
- Menus base de SmartWMS.
- Seed data inicial de configuracion.
- Endpoints API para consultar plan y ejecutar instalacion.

## Endpoints

```http
GET /api/schema/plan
POST /api/schema/install
```

## Request de instalacion

```json
{
  "companyCode": "SBODEMO",
  "dryRun": true,
  "stopOnFirstError": true,
  "installTables": true,
  "installUdos": true,
  "installPermissions": true,
  "installMenus": true,
  "installSeedData": true
}
```

## Nota tecnica

La implementacion actual usa `ISapSchemaService` como contrato de instalacion real y `NullSapSchemaService` como adaptador seguro para compilar y probar sin SAP instalado.

El siguiente paso productivo es reemplazar `NullSapSchemaService` por un adaptador DI API real que implemente:

- `UserTablesMD`
- `UserFieldsMD`
- `UserObjectsMD`
- `UserPermissionTree`
- Menus
- Insercion/actualizacion de seed data en UDTs

