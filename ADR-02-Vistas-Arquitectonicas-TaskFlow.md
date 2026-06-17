# ADR-02: Vistas Arquitectónicas del Sistema TaskFlow

| Campo   | Valor              |
|---------|--------------------| 
| Autor   | Enrique Zavala     |
| Fecha   | 05/06/2026         |
| Estado  | Aprobado           |
| Basado en | ADR-01           |

---

## Contexto

Como parte del desarrollo de **TaskFlow**, una aplicación de gestión de tareas personales dirigida a estudiantes, se requiere documentar formalmente las vistas arquitectónicas del sistema. Esto permite comunicar la estructura y el comportamiento del sistema desde distintas perspectivas, facilitando su comprensión, mantenimiento y evolución futura.

Las vistas se aplican sobre la arquitectura en capas definida en ADR-01 (Controller → Service → Repository → Base de Datos), desarrollada con C# y ASP.NET Core.

---

## Decisión

Se documentan las **4 vistas arquitectónicas del modelo 4+1** aplicadas al proyecto TaskFlow:

1. **Vista Lógica** – Estructura interna de clases y capas.
2. **Vista de Procesos** – Flujo de ejecución en tiempo de ejecución.
3. **Vista de Despliegue** – Infraestructura física donde corre el sistema.
4. **Vista Física** – Distribución de componentes y módulos del sistema.

Además, se implementó una **capa de API REST** que expone los servicios de TaskFlow mediante HTTP, permitiendo que clientes externos (web, móvil, etc.) consuman los servicios sin acoplamiento a la implementación interna.

---

## ¿Por qué estas vistas?

- El modelo 4+1 es un estándar reconocido para documentar arquitecturas de software.
- Permiten entender el sistema desde distintos ángulos: desarrollador, operaciones y stakeholders.
- Los diagramas en Mermaid son compatibles con GitHub y no requieren herramientas externas.
- La capa API REST facilita la integración con múltiples clientes y sigue el patrón REST bien conocido.
- Complementan la arquitectura en capas ya definida en ADR-01.

---

## Vista 1: Vista Lógica

Muestra las principales clases, capas y sus relaciones dentro del sistema TaskFlow, incluyendo la capa API REST.

\`\`\`mermaid
classDiagram
    class TaskController {
        +GetAll() IActionResult
        +GetById(id) IActionResult
        +Create(taskDto) IActionResult
        +Update(id, taskDto) IActionResult
        +Delete(id) IActionResult
    }

    class TaskApiController {
        +GetAll() IActionResult
        +GetById(id) IActionResult
        +CreateTask(taskDto) IActionResult
        +UpdateTask(id, taskDto) IActionResult
        +DeleteTask(id) IActionResult
    }

    class TaskService {
        +GetAllTasks() List~TaskDto~
        +GetTaskById(id) TaskDto
        +CreateTask(taskDto) bool
        +UpdateTask(id, taskDto) bool
        +DeleteTask(id) bool
    }

    class TaskRepository {
        +FindAll() List~Task~
        +FindById(id) Task
        +Save(task) bool
        +Update(task) bool
        +Delete(id) bool
    }

    class Task {
        +int Id
        +string Title
        +string Description
        +bool IsCompleted
        +DateTime CreatedAt
        +DateTime? DueDate
    }

    class TaskDto {
        +string Title
        +string Description
        +bool IsCompleted
        +DateTime? DueDate
    }

    class AppDbContext {
        +DbSet~Task~ Tasks
    }

    TaskController --> TaskService : usa
    TaskApiController --> TaskService : usa
    TaskService --> TaskRepository : usa
    TaskRepository --> AppDbContext : usa
    TaskRepository --> Task : maneja
    TaskController --> TaskDto : recibe/retorna
    TaskApiController --> TaskDto : recibe/retorna
    TaskService --> TaskDto : procesa
\`\`\`

---

## Vista 2: Vista de Procesos

Muestra el flujo de ejecución cuando un cliente (navegador o aplicación móvil) realiza una petición a la API REST para crear una nueva tarea.

\`\`\`mermaid
sequenceDiagram
    actor Usuario
    participant Browser as Cliente HTTP<br/>(Navegador / Postman)
    participant ApiController as TaskApiController
    participant Service as TaskService
    participant Repository as TaskRepository
    participant DB as Base de Datos<br/>(PostgreSQL)

    Usuario->>Browser: Hacer petición POST
    Browser->>ApiController: POST /api/tasks (TaskDto)
    ApiController->>ApiController: Valida modelo (ModelState)
    alt Modelo inválido
        ApiController-->>Browser: 400 Bad Request (JSON)
    else Modelo válido
        ApiController->>Service: CreateTask(taskDto)
        Service->>Service: Mapea DTO a entidad Task
        Service->>Repository: Save(task)
        Repository->>DB: INSERT INTO Tasks
        DB-->>Repository: Filas afectadas
        Repository-->>Service: true / false
        Service-->>ApiController: true / false
        alt Guardado exitoso
            ApiController-->>Browser: 201 Created (JSON)
        else Error al guardar
            ApiController-->>Browser: 500 Internal Server Error
        end
    end
    Browser-->>Usuario: Resultado (JSON)
\`\`\`

---

## Vista 3: Vista de Despliegue

Muestra cómo se despliega TaskFlow en el entorno de ejecución del estudiante, con la capa API REST exponiendo los servicios.

\`\`\`mermaid
graph TD
    subgraph PC_Estudiante["💻 PC del Estudiante (Windows / macOS)"]
        subgraph DotNet["Proceso: ASP.NET Core Runtime"]
            MVC["TaskFlow MVC<br/>(Controllers/Views)"]
            API["TaskFlow API REST<br/>(Api Controllers)"]
            Services["Task Services<br/>(Lógica de negocio)"]
        end
        subgraph Datos["Servidor de Base de Datos"]
            DB[("PostgreSQL<br/>(puerto 5432)")]
        end
        MVC --> Services
        API --> Services
        Services -- "Entity Framework Core<br/>(Npgsql Provider)" --> DB
    end

    subgraph Cliente["Clientes HTTP"]
        Browser["Navegador<br/>(MVC Web)"]
        Postman["Postman / cURL<br/>(Pruebas API REST)"]
        Mobile["App Móvil<br/>(Futuro)"]
    end

    Browser -- "HTTP :5000" --> MVC
    Postman -- "HTTP :5000" --> API
    Mobile -- "HTTP :5000" --> API
\`\`\`

---

## Vista 4: Vista Física (Componentes)

Muestra los módulos y componentes del sistema, incluyendo la capa API REST.

\`\`\`mermaid
graph LR
    subgraph TaskFlow_Project["📦 Proyecto TaskFlow (ASP.NET Core)"]

        subgraph MvcControllers["📂 Controllers (MVC)"]
            TC[TaskController]
        end

        subgraph ApiControllers["📂 Api/Controllers (REST)"]
            TAC[TaskApiController]
        end

        subgraph Services["📂 Services"]
            TS[TaskService]
            ITS[ITaskService]
        end

        subgraph Repositories["📂 Repositories"]
            TR[TaskRepository]
            ITR[ITaskRepository]
        end

        subgraph Models["📂 Models"]
            TM[Task]
        end

        subgraph DTOs["📂 DTOs"]
            TD[TaskDto]
        end

        subgraph Data["📂 Data"]
            CTX[AppDbContext]
        end

        subgraph Config["📂 Configuración"]
            PP[Program.cs]
            AS[appsettings.json]
        end
    end

    subgraph Externo["🗄️ Base de Datos"]
        DB[("PostgreSQL")]
    end

    TC --> TS
    TAC --> TS
    TS --> TR
    TR --> CTX
    CTX --> DB
\`\`\`

---

## Implementación: Capa API REST

La capa API REST se implementó en `TaskFlow.Api` como un proyecto separado que expone los servicios del negocio mediante controladores REST siguiendo el patrón de convención sobre configuración de ASP.NET Core.

### Estructura de la API REST:

\`\`\`
TaskFlow.Api/
├── Controllers/
│   ├── TaskApiController.cs          # Endpoints: GET, POST, PUT, DELETE
│   ├── TaskCategoryApiController.cs  # Endpoints para categorías
│   └── TaskStatusApiController.cs    # Endpoints para estados
├── Dtos/
│   ├── CreateTaskDto.cs
│   ├── UpdateTaskDto.cs
│   └── TaskResponseDto.cs
├── Mapping/
│   └── MappingProfile.cs             # AutoMapper para DTO ↔ Entidad
└── Program.cs                        # Registro de servicios y CORS
\`\`\`

### Endpoints REST Implementados:

| Verbo HTTP | Endpoint              | Descripción                |
|------------|-----------------------|---------------------------|
| GET        | /api/tasks            | Obtener todas las tareas   |
| GET        | /api/tasks/{id}       | Obtener tarea por ID       |
| POST       | /api/tasks            | Crear nueva tarea          |
| PUT        | /api/tasks/{id}       | Actualizar tarea existente |
| DELETE     | /api/tasks/{id}       | Eliminar tarea             |

### Configuración CORS:

En `Program.cs` se configuró CORS para permitir acceso desde clientes web:

\`\`\`csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

app.UseCors("AllowAll");
\`\`\`

### Mapeo DTO:

Se utilizó **AutoMapper** para mapear entre DTOs y entidades:

\`\`\`csharp
public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }
}
\`\`\`

---

## Beneficios de esta Arquitectura

1. **Separación de responsabilidades:** MVC maneja presentación web, API maneja datos.
2. **Reutilización de servicios:** Tanto MVC como API usan la misma capa de ServiceS.
3. **Escalabilidad:** Fácil agregar nuevos clientes (móvil, escritorio) sin modificar lógica.
4. **Testing:** Cada capa puede testearse independientemente.
5. **Documentación automática:** Swagger genera documentación de la API automáticamente.

---

## Conclusión

La implementación de vistas arquitectónicas basadas en el modelo 4+1, junto con una capa API REST bien definida, proporciona a TaskFlow una arquitectura sólida, mantenible y escalable que permite evolucionar el sistema para soportar múltiples clientes en el futuro.
