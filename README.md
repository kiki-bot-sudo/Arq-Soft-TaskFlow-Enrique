# TaskFlow API - Agenda de Actividades Diarias

**Proyecto:** TaskFlow - Gestor de Actividades Diarias  
**Autor:** Enrique Zavala  
**Tecnología:** ASP.NET Core 8 + SQL Server  
**Arquitectura:** Multi-capa (Domain → Application → Infrastructure → API)

---

## 📋 Descripción

**TaskFlow** es una API REST para gestionar actividades diarias personalizadas. Permite a usuarios crear, organizar y rastrear sus actividades del día, agrupadas por prioridad (Alta, Normal, Baja) y categoría.

### Funcionalidades Principales:
- ✅ Crear, leer, actualizar y eliminar actividades diarias
- ✅ Organizar actividades por categoría y prioridad
- ✅ Crear sub-tareas dentro de cada actividad
- ✅ Marcar actividades como completadas
- ✅ Filtrar actividades por fecha
- ✅ API REST completamente documentada con Swagger

---

## 🏗️ Arquitectura en Capas

TaskFlow sigue una **arquitectura de 4 capas** que separa responsabilidades y facilita mantenimiento:

```
┌─────────────────────────────────────┐
│        TaskFlow.Api (REST)          │  ← Presentación (Controllers)
├─────────────────────────────────────┤
│    TaskFlow.Application (Servicios) │  ← Lógica de aplicación
├─────────────────────────────────────┤
│   TaskFlow.Infrastructure (Datos)   │  ← Acceso a datos + EF Core
├─────────────────────────────────────┤
│      TaskFlow.Domain (Modelos)      │  ← Núcleo del negocio
└─────────────────────────────────────┘
```

### 1️⃣ **TaskFlow.Domain** (Núcleo de Negocio)
Contiene los modelos principales sin dependencias externas:
- `Activity.cs` - Modelo de actividad diaria
- `Task.cs` - Modelo de tarea dentro de una actividad

**Propósito:** Definir la estructura de datos pura del dominio.

### 2️⃣ **TaskFlow.Application** (Lógica de Aplicación)
Servicios que orquestan la lógica:
- `IActivityService` / `ActivityService` - Operaciones sobre actividades
- `ITaskService` / `TaskService` - Operaciones sobre tareas

**Propósito:** Implementar reglas de negocio sin conocer detalles de persistencia.

### 3️⃣ **TaskFlow.Infrastructure** (Acceso a Datos)
Implementaciones concretas para persistencia:
- `TaskFlowDbContext` - Contexto de EF Core
- `ActivityRepository` / `IActivityRepository` - CRUD de actividades
- `TaskRepository` / `ITaskRepository` - CRUD de tareas
- **Base de datos:** SQL Server (LocalDB o servidor)

**Propósito:** Interactuar con la base de datos mediante Entity Framework Core.

### 4️⃣ **TaskFlow.Api** (Presentación - REST)
Controladores que exponen endpoints HTTP:
- `ActivityController` - Endpoints `/api/activity`
- `TaskController` - Endpoints `/api/activity/{activityId}/task`
- DTOs para serialización JSON

**Propósito:** Recibir peticiones HTTP y retornar respuestas JSON.

---

## 🔗 Flujo de una Petición

```
Cliente HTTP (Postman, React, etc.)
    ↓
[ActivityController] ← Valida y recibe la petición
    ↓
[ActivityService] ← Aplica reglas de negocio
    ↓
[ActivityRepository] ← Accede a datos
    ↓
[SQL Server] ← Persiste información
    ↓
Respuesta JSON al cliente
```

---

## 📡 Endpoints API REST

### Actividades

| Método | Ruta | Descripción |
|--------|------|-------------|
| `GET` | `/api/activity/today` | Obtener actividades de hoy |
| `GET` | `/api/activity/date/{date}` | Actividades de una fecha específica |
| `GET` | `/api/activity/{id}` | Obtener actividad por ID |
| `POST` | `/api/activity` | Crear nueva actividad |
| `PUT` | `/api/activity/{id}` | Actualizar actividad |
| `DELETE` | `/api/activity/{id}` | Eliminar actividad |

### Tareas

| Método | Ruta | Descripción |
|--------|------|-------------|
| `GET` | `/api/activity/{activityId}/task` | Obtener tareas de una actividad |
| `GET` | `/api/activity/{activityId}/task/{id}` | Obtener tarea por ID |
| `POST` | `/api/activity/{activityId}/task` | Crear tarea en actividad |
| `PUT` | `/api/activity/{activityId}/task/{id}` | Actualizar tarea |
| `DELETE` | `/api/activity/{activityId}/task/{id}` | Eliminar tarea |

---

## 🗄️ Modelos de Datos

### Activity
```csharp
public class Activity
{
    public int Id { get; set; }
    public string Title { get; set; }              // Título de la actividad
    public string Description { get; set; }        // Descripción detallada
    public DateTime Date { get; set; }             // Fecha de la actividad
    public string Category { get; set; }           // Ej: "Trabajo", "Personal"
    public string Priority { get; set; }           // "Low", "Normal", "High"
    public bool IsCompleted { get; set; }          // ¿Está completada?
    public DateTime CreatedAt { get; set; }        // Fecha de creación
    public DateTime? UpdatedAt { get; set; }       // Última actualización
    public ICollection<Task> Tasks { get; set; }   // Sub-tareas
}
```

### Task
```csharp
public class Task
{
    public int Id { get; set; }
    public int ActivityId { get; set; }            // Referencia a Activity
    public string Title { get; set; }              // Título de la tarea
    public string Description { get; set; }        // Descripción
    public bool IsCompleted { get; set; }          // ¿Está completada?
    public DateTime? DueTime { get; set; }         // Hora límite
    public DateTime CreatedAt { get; set; }        // Fecha de creación
    public Activity Activity { get; set; }         // Navegación
}
```

---

## 🚀 Instalación y Ejecución

### Requisitos Previos
- .NET 8 SDK
- SQL Server (LocalDB o instalado localmente)
- Visual Studio 2022 o VS Code

### Pasos

1. **Clonar el repositorio:**
   ```bash
   git clone https://github.com/kiki-bot-sudo/Arq-Soft-TaskFlow-Enrique.git
   cd Arq-Soft-TaskFlow-Enrique
   git checkout api
   ```

2. **Abrir solución en Visual Studio:**
   ```bash
   start TaskFlow.sln
   ```

3. **Configurar cadena de conexión** en `TaskFlow.Api/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\mssqllocaldb;Database=TaskFlowDb;Trusted_Connection=true;"
     }
   }
   ```

4. **Crear base de datos con migraciones:**
   ```bash
   dotnet ef database update --project TaskFlow.Infrastructure --startup-project TaskFlow.Api
   ```

5. **Ejecutar la API:**
   - En Visual Studio: Click en el botón ▶ **Run**
   - O en terminal: `dotnet run --project TaskFlow.Api`

6. **Acceder a Swagger:** https://localhost:5001/swagger/index.html

---

## 🧪 Pruebas con Swagger

1. Abre https://localhost:5001/swagger
2. Expande cualquier endpoint
3. Click en **"Try it out"**
4. Ingresa los parámetros/body
5. Click en **"Execute"**

### Ejemplo: Crear Actividad
```json
{
  "title": "Estudiar arquitectura en capas",
  "description": "Aprender el patrón Domain → Application → Infrastructure → API",
  "date": "2026-06-17",
  "category": "Educación",
  "priority": "High"
}
```

---

## 📁 Estructura del Proyecto

```
TaskFlow.Api/
├── TaskFlow.Domain/
│   └── Models/
│       ├── Activity.cs
│       └── Task.cs
├── TaskFlow.Application/
│   ├── Interfaces/
│   │   ├── IActivityService.cs
│   │   └── ITaskService.cs
│   └── Services/
│       ├── ActivityService.cs
│       └── TaskService.cs
├── TaskFlow.Infrastructure/
│   ├── Data/
│   │   └── TaskFlowDbContext.cs
│   ├── Interfaces/
│   │   ├── IActivityRepository.cs
│   │   └── ITaskRepository.cs
│   └── Repositories/
│       ├── ActivityRepository.cs
│       └── TaskRepository.cs
├── TaskFlow.Api/
│   ├── Controllers/
│   │   ├── ActivityController.cs
│   │   └── TaskController.cs
│   ├── DTOs/
│   │   ├── CreateActivityDto.cs
│   │   ├── UpdateActivityDto.cs
│   │   ├── CreateTaskDto.cs
│   │   └── UpdateTaskDto.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── TaskFlow.Api.csproj
└── TaskFlow.sln
```

---

## 🎯 Patrones Implementados

### ✅ Repository Pattern
Abstrae el acceso a datos mediante interfaces `IActivityRepository` e `ITaskRepository`, permitiendo cambiar la implementación sin afectar servicios.

### ✅ Dependency Injection
En `Program.cs` se registran servicios y repositorios para inyección automática:
```csharp
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
```

### ✅ Entity Framework Core
ORM que mapea automáticamente clases de C# a tablas SQL Server.

### ✅ CORS Habilitado
Permite que clientes desde cualquier origen accedan a la API (útil para React, Postman, etc.).

### ✅ DTOs (Data Transfer Objects)
Separa la estructura de la API (`CreateActivityDto`) del modelo interno (`Activity`), protegiendo la lógica de negocio.

---

## 🔐 Seguridad (Futuro)

- Agregar autenticación JWT
- Implementar autorización por usuario
- Validación de datos en DTOs
- Rate limiting

---

## 📝 Cláusula de Uso de IA

Durante el desarrollo de **TaskFlow.Api**, se utilizó **Claude (Anthropic)** como herramienta asistente para:

- **Generación de estructura:** Crear la arquitectura base en 4 capas (Domain, Application, Infrastructure, Api) con patrones SOLID.
- **Implementación de repositories:** Código de `ActivityRepository` y `TaskRepository` usando Entity Framework Core.
- **Controllers REST:** Generación de `ActivityController` y `TaskController` con validaciones básicas.
- **Configuración inicial:** Setup de `Program.cs` con inyección de dependencias, CORS y conexión a SQL Server.
- **DTOs:** Definición de `CreateActivityDto`, `UpdateActivityDto`, etc.
- **Documentación:** Generación de comentarios y documentación de endpoints.

La **comprensión de conceptos**, **decisiones arquitectónicas**, **integración de componentes** y **pruebas finales** fueron responsabilidad del alumno (Enrique Zavala). La IA actuó como acelerador de desarrollo, permitiendo enfoque en aprendizaje de patrones y mejores prácticas.

---

## 👨‍💻 Autor

**Enrique Zavala**

- Alumno de Tecnológico de Software
- Estudiante de Arquitectura de Software
- GitHub: [@kiki-bot-sudo](https://github.com/kiki-bot-sudo)
- Email: enrique.zavala@tecdesoftware.edu.mx

---

## 📄 Licencia

Este proyecto es de uso educativo. Libre para modificar y distribuir con fines académicos.

---

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Por favor:
1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/nueva-feature`)
3. Commit tus cambios (`git commit -m 'feat: descripción'`)
4. Push a la rama (`git push origin feature/nueva-feature`)
5. Abre un Pull Request

---

## 📞 Contacto

Para preguntas o sugerencias sobre TaskFlow:
- GitHub Issues: [Reportar problema](https://github.com/kiki-bot-sudo/Arq-Soft-TaskFlow-Enrique/issues)
- Email: enrique.zavala@tecdesoftware.edu.mx

---

**Última actualización:** Junio 2026
