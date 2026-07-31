# TaskFlow — Agenda personal

TaskFlow es una agenda web escolar para organizar tareas personales. Incluye cuentas seguras, tareas privadas por usuario y subtareas.

## Objetivo

Aplicar .NET, ASP.NET Core, Entity Framework Core, arquitectura por capas y despliegue en Azure en una solución pequeña, entendible y presentable.

## Tecnologías

- .NET 8 y ASP.NET Core Web API.
- Entity Framework Core 8.
- SQL Server / Azure SQL Database.
- HTML, CSS y JavaScript nativo servido desde `TaskFlow.Api/wwwroot`.
- xUnit y Moq.
- Swagger/OpenAPI.

## Requisitos

- .NET 8 SDK.
- SQL Server Express, LocalDB o SQL Server.
- Opcional: Visual Studio 2022.

## Instalación y base de datos

```powershell
git clone https://github.com/kiki-bot-sudo/Arq-Soft-TaskFlow-Enrique.git
cd Arq-Soft-TaskFlow-Enrique
dotnet restore
dotnet ef database update --project TaskFlow.Infrastructure --startup-project TaskFlow.Api
```

La conexión de desarrollo está en `TaskFlow.Api/appsettings.json`. Se recomienda sobrescribirla con Secret Manager:

```powershell
dotnet user-secrets init --project TaskFlow.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost\SQLEXPRESS;Database=TaskFlowDb;Trusted_Connection=True;TrustServerCertificate=True" --project TaskFlow.Api
```

## Ejecución

```powershell
dotnet run --project TaskFlow.Api
```

- Agenda: `https://localhost:57306`
- Swagger: `https://localhost:57306/swagger`
- Salud: `https://localhost:57306/health`

## Funcionalidades

- CRUD completo de tareas.
- Registro, inicio y cierre de sesión con ASP.NET Core Identity.
- Tareas, estadísticas y filtros aislados por usuario.
- Subtareas que pueden agregarse, completarse y eliminarse.
- Título obligatorio, descripción, fecha de creación y fecha límite.
- Prioridad baja, media y alta.
- Estado pendiente, completado y vencido.
- Búsqueda por título, filtros y ordenamiento.
- Estadísticas de total, pendientes, completadas y vencidas.
- Interfaz responsive, confirmación al eliminar y mensajes de resultado.
- Rutas antiguas de actividades conservadas por compatibilidad.

Identity almacena hashes seguros, nunca contraseñas en texto. Las tareas anteriores a la migración conservan `UserId = null` y no se asignan automáticamente a una cuenta.

## Estructura

```text
TaskFlow.Domain/          Entidades y builders
TaskFlow.Application/     Servicios y reglas de aplicación
TaskFlow.Infrastructure/  DbContext, repositorios y migraciones
TaskFlow.Api/             API, DTO, middleware y frontend en wwwroot
TaskFlow.Tests/           Pruebas con xUnit y Moq
docs/                     ADR, ATAM, línea de productos y diagramas
```

## Pruebas

```powershell
dotnet build TaskFlow.sln --configuration Release
dotnet test TaskFlow.Tests --configuration Release
```

## Despliegue básico en Azure

1. Crear una Azure SQL Database y permitir la conexión desde Azure App Service.
2. Crear un App Service para .NET 8.
3. En **Configuration**, agregar `ConnectionStrings__DefaultConnection` con la conexión de Azure SQL.
4. Aplicar migraciones antes del primer arranque:

   ```powershell
   dotnet ef database update --project TaskFlow.Infrastructure --startup-project TaskFlow.Api --connection "<cadena-azure-sql>"
   ```

5. Publicar:

   ```powershell
   dotnet publish TaskFlow.Api -c Release -o ./publish
   az webapp deploy --resource-group <grupo> --name <app> --src-path ./publish.zip --type zip
   ```

6. Comprobar `/health`, la página principal y el CRUD.

Para producción, la cadena de conexión debe guardarse en App Service o Key Vault, nunca en Git.

## Documentación

- [Decisiones arquitectónicas](docs/adr/)
- [Evaluación ATAM simplificada](docs/ATAM.md)
- [Línea de productos](docs/LineaDeProductos.md)
- [Diagramas](docs/Diagramas.md)
- [Guía detallada de Azure](docs/AZURE_DEPLOYMENT.md)

## URL pública y QR

- Aplicación y API: <https://taskflow-api-enriq73026.azurewebsites.net>
- Salud: <https://taskflow-api-enriq73026.azurewebsites.net/health>

Para regenerar el QR:

```powershell
python -m pip install "qrcode[pil]"
.\scripts\generate-qr.ps1 -Url "https://taskflow-api-enriq73026.azurewebsites.net"
```

El resultado se guarda localmente en `docs/taskflow-azure-qr.png`.

