# Fase 11.0 — SmartWMS UI Foundation

## Objetivo

Crear la base de interfaz de usuario para SmartWMS 2.0 usando Blazor Web App .NET 8 con interactividad Server.

## Componentes agregados

- Configuración de `HttpClient` nombrado `SmartWmsApi`.
- Configuración base de autenticación con JWT almacenado en `localStorage`.
- `AuthTokenMessageHandler` para enviar Bearer Token al backend.
- `SmartWmsAuthenticationStateProvider`.
- `AuthService` y `SmartWmsApiClient`.
- `UiStateService` para estado visual global.
- `LoadingOverlay`.
- `StatusBadge`.
- Layout base SmartWMS.
- Menú lateral operativo.
- Pantallas base:
  - Login
  - Dashboard
  - Inventory
  - Picking
  - Packing
  - Shipping
  - Devices

## Configuración API

El archivo `appsettings.json` contiene:

```json
{
  "SmartWmsApi": {
    "BaseUrl": "https://localhost:7001/"
  }
}
```

Ajustar `BaseUrl` al puerto real donde se ejecute `DETechOne.SmartWMS.API`.

## Siguiente fase

Fase 11.1 — Login UI & Session Management
