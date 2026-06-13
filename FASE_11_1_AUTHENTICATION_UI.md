# Fase 11.1 - Authentication UI

## Objetivo

Completar la experiencia visual de autenticacion para SmartWMS 2.0 usando el backend JWT existente.

## Alcance implementado

- Login UI con validacion de campos.
- Soporte para `returnUrl`.
- Redireccion automatica cuando el usuario no esta autenticado.
- Menu lateral dependiente del estado de autenticacion.
- Topbar con usuario autenticado y boton de salida.
- Pagina de perfil para inspeccion de claims JWT.
- Validacion basica de expiracion del token JWT.
- Proteccion de paginas operativas con `[SmartWmsAuthorize]`.
- Manejo de respuestas `401` del API sin convertirlas en error de conectividad.
- Redireccion de `/` hacia `/login` o `/dashboard` segun estado de sesion.
- Eliminacion de paginas demo del template (`/counter`, `/weather`).

## Paginas protegidas

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

## Verificacion automatizada ejecutada

Fecha: 2026-06-13

Comandos:

```powershell
dotnet build DETechOne.SmartWMS.sln
dotnet test DETechOne.SmartWMS.sln --no-build
```

Resultado:

- Build correcto con `0` advertencias y `0` errores.
- Tests correctos: `13/13`.

Smoke HTTP con API y Web levantadas localmente:

- `POST http://localhost:5289/api/auth/login` con credenciales validas: `HTTP 200`.
- `POST http://localhost:5289/api/auth/login` con credenciales invalidas: `HTTP 401`.
- `GET http://localhost:5186/`: `HTTP 200`.
- `GET http://localhost:5186/login`: `HTTP 200`.
- `GET http://localhost:5186/dashboard`: `HTTP 200`.
- `GET http://localhost:5186/profile`: `HTTP 200`.
- `GET http://localhost:5186/counter`: `HTTP 404`.
- `GET http://localhost:5186/weather`: `HTTP 404`.

Nota tecnica: en Blazor Server, las rutas protegidas pueden devolver `HTTP 200` en una peticion directa porque la validacion de sesion y la redireccion visual se completan durante el circuito interactivo del cliente.

## Verificacion manual pendiente

Ejecutar API:

```powershell
dotnet run --project .\DETechOne.SmartWMS.API\DETechOne.SmartWMS.API.csproj --launch-profile https
```

Ejecutar Web:

```powershell
dotnet run --project .\DETechOne.SmartWMS.Web\DETechOne.SmartWMS.Web.csproj --launch-profile https
```

Casos manuales:

1. Entrar a `http://localhost:5186/dashboard` sin sesion.
2. Confirmar redireccion visual a `/login?returnUrl=...`.
3. Iniciar sesion con `admin / SmartWMS#2026`.
4. Confirmar redireccion a `/dashboard`.
5. Confirmar que el menu lateral muestra Dashboard, Inventory, Picking, Packing, Shipping, Devices, Perfil y Salir.
6. Entrar a `/profile` y confirmar que se muestran claims del JWT.
7. Ejecutar Salir.
8. Confirmar redireccion a `/login`.
9. Confirmar que el token se remueve de `localStorage`.

## Limitacion de esta corrida

La verificacion visual con navegador integrado no se pudo ejecutar en el entorno local porque el runner rechazo la creacion del proceso del navegador. La validacion automatizada se limito a build, tests, smoke HTTP de API y smoke HTTP de Web.
