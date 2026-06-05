# ADR-02: Vistas Arquitectónicas del Sistema TaskFlow

| Campo   | Valor              |
|---------|--------------------|
| Autor   | Enrique Zavala     |
| Fecha   | 05/06/2026         |
| Estado  | Propuesto          |
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

---

## ¿Por qué estas vistas?

- El modelo 4+1 es un estándar reconocido para documentar arquitecturas de software.
- Permiten entender el sistema desde distintos ángulos: desarrollador, operaciones y stakeholders.
- Los diagramas en Mermaid son compatibles con GitHub y no requieren herramientas externas.
- Complementan la arquitectura en capas ya definida en ADR-01.

---

## Vista 1: Vista Lógica

Muestra las principales clases, capas y sus relaciones dentro del sistema TaskFlow.

```mermaid
classDiagram
    class TaskController {
        +GetAll() IActionResult
        +GetById(id) IActionResult
        +Create(taskDto) IActionResult
        +Update(id, taskDto) IActionResult
        +Delete(id) IActionResult
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
    TaskService --> TaskRepository : usa
    TaskRepository --> AppDbContext : usa
    TaskRepository --> Task : maneja
    TaskController --> TaskDto : recibe/retorna
    TaskService --> TaskDto : procesa
```

---

## Vista 2: Vista de Procesos

Muestra el flujo de ejecución cuando un usuario crea una nueva tarea en TaskFlow.

```mermaid
sequenceDiagram
    actor Usuario
    participant Controller as TaskController
    participant Service as TaskService
    participant Repository as TaskRepository
    participant DB as Base de Datos (PostgreSQL)

    Usuario->>Controller: POST /api/tasks (TaskDto)
    Controller->>Controller: Valida modelo (ModelState)
    alt Modelo inválido
        Controller-->>Usuario: 400 Bad Request
    else Modelo válido
        Controller->>Service: CreateTask(taskDto)
        Service->>Service: Mapea DTO a entidad Task
        Service->>Repository: Save(task)
        Repository->>DB: INSERT INTO Tasks
        DB-->>Repository: Filas afectadas
        Repository-->>Service: true / false
        Service-->>Controller: true / false
        alt Guardado exitoso
            Controller-->>Usuario: 201 Created
        else Error al guardar
            Controller-->>Usuario: 500 Internal Server Error
        end
    end
```

---

## Vista 3: Vista de Despliegue

Muestra cómo se despliega TaskFlow en el entorno de ejecución del estudiante (entorno local de desarrollo), con PostgreSQL como motor de base de datos.

```mermaid
graph TD
    subgraph PC_Estudiante["💻 PC del Estudiante (Windows / macOS)"]
        subgraph DotNet["Proceso: ASP.NET Core Runtime"]
            API["TaskFlow API\n(ASP.NET Core Web API)"]
        end
        subgraph Datos["Servidor de Base de Datos"]
            DB[("PostgreSQL\n(puerto 5432)")]
        end
        API -- "Entity Framework Core\n(Npgsql Provider)" --> DB
    end

    subgraph Cliente["Navegador / Cliente HTTP"]
        Browser["Swagger UI / Postman\n(pruebas de la API)"]
    end

    Browser -- "HTTP :5000" --> API
```

---

## Vista 4: Vista Física (Componentes)

Muestra los módulos y componentes del sistema y cómo están organizados dentro del proyecto.

```mermaid
graph LR
    subgraph TaskFlow_Project["📦 Proyecto TaskFlow (ASP.NET Core)"]

        subgraph Controllers["📂 Controllers"]
            TC[TaskController]
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

    TC --> ITS
    ITS -.implementa.- TS
    TS --> ITR
    ITR -.implementa.- TR
    TR --> CTX
    CTX --> DB
    TC --> TD
    TS --> TM
    PP --> TC
    PP --> CTX
    AS --> CTX
```

---

## Consecuencias

**Ventajas:**
- Las vistas documentan el sistema desde múltiples perspectivas, mejorando la comprensión.
- Los diagramas Mermaid se renderizan directamente en GitHub sin herramientas adicionales.
- Facilitan la incorporación de nuevos desarrolladores o revisores al proyecto.
- Sirven como base para futuras decisiones de arquitectura.

**Desventajas:**
- Requiere mantener los diagramas actualizados conforme evoluciona el código.
- La vista de despliegue es simple por tratarse de un entorno local de desarrollo.

---

## Alternativas consideradas

| Alternativa | Razón de descarte |
|---|---|
| UML con herramienta gráfica (Lucidchart, etc.) | No integra bien con el repositorio; requiere exportar imágenes |
| Draw.io en XML | Más complejo de versionar; Mermaid es más legible en texto plano |
| No documentar vistas | Incumple el objetivo de la práctica y dificulta el mantenimiento |

---

## Declaración de uso de IA

Para la elaboración de este ADR se utilizó inteligencia artificial (Claude de Anthropic) como apoyo en la redacción y generación de los diagramas Mermaid. La revisión, validación del contenido y decisiones arquitectónicas son responsabilidad del autor.

---

*ADR-02 — TaskFlow | Enrique Zavala | Junio 2026*
