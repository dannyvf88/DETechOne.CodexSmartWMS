# Fase 11.1 — Authentication UI

## Objetivo

Completar la experiencia visual de autenticación para SmartWMS 2.0 usando el backend JWT existente.

## Alcance implementado

- Login UI con validación de campos.
- Soporte para `returnUrl`.
- Redirección automática cuando el usuario no está autenticado.
- Menú lateral dependiente del estado de autenticación.
- Topbar con usuario autenticado y botón de salida.
- Página de perfil para inspección de claims JWT.
- Validación básica de expiración del token JWT.
- Protección de páginas operativas con `[Authorize]`.

## Páginas protegidas

- `/dashboard`
- `/inventory`
- `/picking`
- `/packing`
- `/shipping`
- `/devices`
- `/profile`

## Usuario temporal de pruebas

```text
admin
SmartWMS#2026
```

## Verificación QA

1. Ejecutar API.
2. Ejecutar Web.
3. Entrar a `/dashboard` sin sesión.
4. Confirmar redirección a `/login`.
5. Iniciar sesión.
6. Confirmar redirección a dashboard.
7. Probar menú Perfil.
8. Probar Salir.
9. Confirmar que el token se remueve de localStorage.
