# TaskFlow API

API RESTful para gestión de actividades y tareas personales. Desarrollada con ASP.NET Core 8 + arquitectura en capas + patrones GoF.

---

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server o SQL Server Express

---

## Configuración y arranque

### 1. Clonar y posicionarse en la rama

```bash
git clone https://github.com/kiki-bot-sudo/Arq-Soft-TaskFlow-Enrique.git
cd Arq-Soft-TaskFlow-Enrique
git checkout api
```

### 2. Configurar la base de datos

Edita `TaskFlow.Api/appsettings.json` con tu cadena de conexión:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=TaskFlowDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Crear la base de datos y aplicar migraciones

```bash
cd TaskFlow.Api
dotnet ef database update --project ../TaskFlow.Infrastructure
```

Esto crea la base de datos `TaskFlowDb` y carga el seed data (3 actividades y 3 tareas de ejemplo).

### 4. Correr la API

```bash
dotnet run --project TaskFlow.Api
```

La API estará disponible en `https://localhost:5001` (o el puerto que muestre la consola).  
Swagger UI en: `https://localhost:5001/` (raíz)

---

## Endpoints principales

### Actividades

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/activity/today` | Actividades de hoy |
| GET | `/api/activity/date/{date}` | Actividades por fecha |
| GET | `/api/activity/{id}` | Actividad por ID |
| POST | `/api/activity` | Crear actividad |
| PUT | `/api/activity/{id}` | Actualizar actividad |
| DELETE | `/api/activity/{id}` | Eliminar actividad |

### Tareas

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/activity/{activityId}/task` | Tareas de una actividad |
| GET | `/api/activity/{activityId}/task/{id}` | Tarea por ID |
| POST | `/api/activity/{activityId}/task` | Crear tarea |
| PUT | `/api/activity/{activityId}/task/{id}` | Actualizar tarea |
| DELETE | `/api/activity/{activityId}/task/{id}` | Eliminar tarea |

> **Nota:** Al marcar todas las tareas de una actividad como completadas, la actividad se marca automáticamente como completada (patrón Observer).

---

## Correr los tests

```bash
dotnet test TaskFlow.Tests
```

20 pruebas unitarias cubriendo todos los patrones GoF implementados.

---

## Patrones de diseño aplicados

Ver [`PATRONES-GOF.md`](./PATRONES-GOF.md) para documentación completa.

| Patrón | Dónde |
|--------|-------|
| Strategy | Ordenamiento de actividades (`Application/Strategies/`) |
| Builder | Construcción de entidades (`Domain/Builders/`) |
| Decorator | Logging de operaciones (`Application/Decorators/`) |
| Observer | Auto-completado de actividades (`Application/Observers/`) |

---

## Estructura del proyecto

```
TaskFlow.sln
├── TaskFlow.Domain/          # Modelos de entidad
├── TaskFlow.Infrastructure/  # Repositorios, DbContext, Migraciones
├── TaskFlow.Application/     # Servicios, Interfaces, Patrones GoF
├── TaskFlow.Api/             # Controllers, DTOs, Middleware
└── TaskFlow.Tests/           # Tests unitarios (xUnit + Moq)
```
