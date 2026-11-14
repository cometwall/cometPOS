# cometPOS

API REST para un sistema de punto de venta (POS), desarrollada con ASP.NET Core (.NET 10) y Entity Framework Core sobre PostgreSQL.

## Características

- **Organización**: gestión de empresas y sucursales (crear, listar, actualizar, desactivar).
- **Identidad y acceso**:
  - Autenticación por usuario/contraseña con emisión de JWT entregado en cookies `HttpOnly` (`access_token`) y refresh tokens con rotación.
  - Roles (`SuperAdmin`, `Admin`, etc.) protegiendo endpoints vía `[Authorize(Roles = ...)]`.
  - Bloqueo de cuenta por intentos fallidos (configurable) y endpoint de desbloqueo manual.
  - Cambio y reseteo de contraseñas.
  - Rate limiting en `login` y `refresh` para mitigar fuerza bruta.
- **Catálogo**: categorías, marcas, unidades de medida, productos, variantes y atributos.
- **Inventario**: lotes, series, movimientos e inventario por sucursal (modelado en EF Core).
- **Retornables**: control de envases/retornables y sus movimientos (modelado en EF Core).
- **Auditoría**: interceptor de EF Core (`AuditInterceptor`) que registra cambios en `AuditLog`, expuesto mediante un endpoint de consulta solo para `SuperAdmin`.
- Persistencia con Entity Framework Core (Npgsql) y migraciones versionadas.

## Estructura

```
cometPOS/
├── Controllers/
│   ├── Auditoria/      Consulta de logs de auditoría
│   ├── Catalogo/       Categorías, marcas, unidades de medida
│   ├── Identidad/      Auth (login/refresh/logout) y usuarios
│   └── Organizacion/   Sucursales
├── Data/               AppDbContext y AuditInterceptor
├── DTOs/                Objetos de transferencia de datos (por módulo)
├── Migrations/          Migraciones de EF Core
├── Models/
│   ├── Auditoria/       AuditLog
│   ├── Catalogo/        Producto, Categoria, Marca, Variantes, Atributos...
│   ├── Identidad/       Usuario, RefreshToken
│   ├── Inventario/      Lotes, series, movimientos de inventario
│   ├── Organizacion/     Empresa, Sucursal
│   └── Retornables/      Retornable, VarianteRetornable, movimientos
├── Services/
│   ├── Auditoria/       AuditService
│   └── Identidad/       TokenService, limpieza de refresh tokens
└── Program.cs
```

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/) (el proyecto usa `Npgsql.EntityFrameworkCore.PostgreSQL`)

## Configuración

1. Clona el repositorio:
   ```bash
   git clone <url-del-repositorio>
   cd cometPOS
   ```
2. Configura la cadena de conexión y los secretos (no se suben al repositorio) desde la carpeta del proyecto (`cometPOS/`):
   ```bash
   cd cometPOS
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=cometpos;Username=<usuario>;Password=<password>"
   dotnet user-secrets set "Jwt:Key" "<clave-secreta-larga>"
   dotnet user-secrets set "Jwt:Issuer" "cometPOS"
   dotnet user-secrets set "Jwt:Audience" "cometPOS"
   ```
3. Configura los orígenes permitidos para CORS (requerido, ya que se envían credenciales/cookies) en `appsettings.json` o `appsettings.Development.json`:
   ```json
   {
     "Cors": {
       "Origenes": ["http://localhost:3000"]
     }
   }
   ```
4. (Opcional) Si la API corre detrás de un proxy inverso, define las IPs confiables:
   ```json
   {
     "ReverseProxy": {
       "KnownProxies": ["127.0.0.1"]
     }
   }
   ```
5. (Opcional) Ajusta los parámetros de bloqueo de cuenta y limpieza de refresh tokens (valores por defecto en `appsettings.json`):
   ```json
   {
     "Lockout": {
       "MaxIntentos": 5,
       "MinutosBloqueo": 15
     },
     "RefreshTokenCleanup": {
       "IntervalInDays": 6,
       "RetencionDias": 7
     }
   }
   ```
6. Aplica las migraciones:
   ```bash
   dotnet ef database update
   ```
7. Ejecuta el proyecto:
   ```bash
   dotnet run
   ```
   Por defecto queda disponible en `http://localhost:5135` (ver `Properties/launchSettings.json`). En entorno de desarrollo, el documento OpenAPI se expone en `/openapi/v1.json`.

## Autenticación

El login (`POST /api/auth/login`) devuelve el access token y el refresh token como cookies `HttpOnly`. Las solicitudes posteriores a endpoints protegidos no necesitan el header `Authorization`: basta con enviar las cookies (`credentials: include` desde el cliente). Usa `POST /api/auth/refresh` para renovar el access token y `POST /api/auth/logout` para cerrar sesión.

## Pruebas de endpoints

El archivo `cometPOS/cometPOS.http` contiene peticiones de ejemplo que puedes ejecutar con la extensión REST Client o con `dotnet` HTTP tooling. También puedes usar [Postman CLI](https://learning.postman.com/docs/postman-cli/postman-cli-overview/) (`postman collection run`, `postman mock`, etc.) si prefieres trabajar con colecciones de Postman.

## Licencia

Define aquí la licencia del proyecto.
