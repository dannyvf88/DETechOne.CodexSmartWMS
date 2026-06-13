# Fase 10.8 — Shipping Engine

## Objetivo

Agregar el motor base de embarque para SmartWMS 2.0, preparando el flujo posterior a Packing y la creación de entregas en SAP Business One.

## Componentes agregados

- Entidades de dominio: `ShippingDocument`, `ShippingLine`.
- Estados: `ShippingStatus`, `ShippingLineStatus`.
- Contratos: DTOs y Requests de Shipping.
- Servicio de aplicación: `IShippingService`, `ShippingService`.
- Repositorio base: `IShippingRepository`, `InMemoryShippingRepository`.
- Adaptador SAP temporal: `ISapDeliveryService`, `NullSapDeliveryService`.
- API: `ShippingController`.

## Endpoints

```text
GET  /api/shipping/open
GET  /api/shipping/{shippingId}
POST /api/shipping
POST /api/shipping/confirm
POST /api/shipping/create-delivery
POST /api/shipping/cancel
```

## Nota productiva

`NullSapDeliveryService` simula la creación de entrega SAP. Para producción debe reemplazarse por una implementación real con DI API o Service Layer.
