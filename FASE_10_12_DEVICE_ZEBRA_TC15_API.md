# Fase 10.12 — Device / Zebra TC15 API

## Objetivo

Agregar la base backend para operación con handhelds Zebra TC15 y scanners Android mediante API REST.

## Alcance implementado

### Registro y control de dispositivos

- Registro de dispositivo.
- Heartbeat operativo.
- Consulta de dispositivo por código.
- Consulta de dispositivos online.

### Scanner sessions

- Inicio de sesión de escaneo.
- Recepción de lecturas desde DataWedge o captura manual.
- Cierre de sesión.
- Cancelación de sesión.
- Consulta de sesión.

## Endpoints agregados

```text
POST /api/devices/register
POST /api/devices/heartbeat
GET  /api/devices/{deviceCode}
GET  /api/devices/online

POST /api/devices/scanner-sessions/start
POST /api/devices/scanner-sessions/scan
POST /api/devices/scanner-sessions/complete
POST /api/devices/scanner-sessions/{scannerSessionId}/cancel
GET  /api/devices/scanner-sessions/{scannerSessionId}
```

## Consideraciones Zebra TC15 / DataWedge

La integración queda preparada para recibir eventos provenientes de Zebra DataWedge. En Android/DataWedge, la app móvil o PWA podrá enviar al endpoint de scan los valores leídos, incluyendo:

- Valor del código.
- Tipo de evento.
- Simbología.
- Fuente.

## Nota técnica

La persistencia actual es en memoria para mantener la fase compilable sin requerir base de datos física ni SAP instalado. En una fase posterior, estos servicios deberán conectarse a tablas reales del esquema SmartWMS.
