# cometPOS

API REST para un sistema de punto de venta (POS) desarrollada con ASP.NET Core (.NET 10) y Entity Framework Core.

## Características

- Gestión de sucursales (crear, listar, actualizar).
- Usuarios y autenticación mediante tokens (`TokenService`).
- Persistencia con Entity Framework Core y migraciones.

## Estructura

```
cometPOS/
├── Controllers/   Endpoints de la API
├── Data/          DbContext
├── DTOs/          Objetos de transferencia de datos
├── Migrations/    Migraciones de EF Core
├── Models/        Entidades (Sucursal, Usuario)
├── Services/      Servicios (generación de tokens)
└── Program.cs
```

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Base de datos compatible con la configuración de EF Core del proyecto

## Configuración

1. Clona el repositorio:
   ```bash
   git clone <url-del-repositorio>
   cd cometPOS
   ```
2. Configura la cadena de conexión y los secretos (no se suben al repositorio):
   ```bash
   cd cometPOS
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<cadena-de-conexion>"
   ```
   Si el servicio de tokens usa claves (por ejemplo `Jwt:Key`), configúralas igual con `user-secrets`.
3. Aplica las migraciones:
   ```bash
   dotnet ef database update
   ```
4. Ejecuta el proyecto:
   ```bash
   dotnet run
   ```

## Pruebas de endpoints

El archivo `cometPOS/cometPOS.http` contiene peticiones de ejemplo.

## Licencia

Define aquí la licencia del proyecto.
