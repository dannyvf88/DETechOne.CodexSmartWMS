# Fase 10.3 — Metadata Installer

Esta fase agrega la base productiva del instalador de metadata de SmartWMS 2.0.

## Incluye

- Contratos de instalación de metadata.
- Plan de metadata SmartWMS inicial.
- Instalador con modo seguro `DryRun`.
- Puerto `ISapMetadataService` para aislar SAP Business One.
- Adaptador SAP temporal `NullSapMetadataService`.
- Registro de servicios con `AddSmartWmsMetadataInstaller()`.
- Endpoints protegidos con JWT:
  - `GET /api/metadata/plan`
  - `POST /api/metadata/install`

## Uso recomendado inicial

1. Ejecutar login con el usuario temporal.
2. Copiar el Bearer Token en Swagger.
3. Consultar `GET /api/metadata/plan`.
4. Ejecutar `POST /api/metadata/install` con `DryRun = true`.

Ejemplo:

```json
{
  "companyCode": "SBODEMO",
  "dryRun": true,
  "stopOnFirstError": true
}
```

## Nota importante

El adaptador real de SAP Business One DI API todavía no se conecta en esta fase. Esta fase deja la arquitectura preparada y segura para reemplazar `NullSapMetadataService` por una implementación DI API/Service Layer en la siguiente fase.
