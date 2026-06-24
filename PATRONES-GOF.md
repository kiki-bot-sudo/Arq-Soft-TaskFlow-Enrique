# Patrones de Diseño GoF — TaskFlow API

| Patrón | Categoría | Archivos |
|--------|-----------|----------|
| Strategy | Comportamiento | `TaskFlow.Application/Strategies/` |
| Builder  | Creación       | `TaskFlow.Domain/Builders/`        |

---

## Patrón 1: Strategy

**Problema que resuelve:** El ordenamiento de actividades estaba hardcodeado dentro de `ActivityService`. Si se quería otro criterio de orden había que modificar el servicio directamente, violando el principio Open/Closed.

**Solución:** Se extrae el algoritmo de ordenamiento a una interfaz `IActivitySortStrategy`. Cada implementación concreta encapsula un criterio distinto. El servicio solo conoce la interfaz.

### Estructura

```
IActivitySortStrategy          ← Contrato (interfaz)
  ├── PriorityDescSortStrategy ← Ordena High > Normal > Low
  └── DateAscSortStrategy      ← Ordena cronológicamente

ActivityService
  └── usa IActivitySortStrategy (por defecto: PriorityDesc)
      └── SetSortStrategy() permite cambiarla en runtime
```

### Diagrama

```mermaid
classDiagram
    class IActivitySortStrategy {
        <<interface>>
        +Sort(activities) IEnumerable~Activity~
    }
    class PriorityDescSortStrategy {
        +Sort(activities) IEnumerable~Activity~
    }
    class DateAscSortStrategy {
        +Sort(activities) IEnumerable~Activity~
    }
    class ActivityService {
        -IActivitySortStrategy _sortStrategy
        +SetSortStrategy(strategy)
        +GetActivitiesByDateAsync(date)
    }

    IActivitySortStrategy <|.. PriorityDescSortStrategy
    IActivitySortStrategy <|.. DateAscSortStrategy
    ActivityService --> IActivitySortStrategy : usa
```

### Antes vs Después

**Antes** (hardcodeado en el servicio):
```csharp
return activities.OrderByDescending(a => a.Priority == "High" ? 0 : a.Priority == "Normal" ? 1 : 2);
```

**Después** (Strategy):
```csharp
// En ActivityService
return _sortStrategy.Sort(activities);

// Para cambiar el criterio desde fuera:
service.SetSortStrategy(new DateAscSortStrategy());
```

---

## Patrón 2: Builder

**Problema que resuelve:** Los controladores construían entidades (`Activity`, `Task`) asignando propiedades una a una directamente. Esto es propenso a errores (olvidar campos, valores nulos) y hace el código difícil de leer.

**Solución:** Se crean `ActivityBuilder` y `TaskBuilder` que encadenan la construcción paso a paso y aplican valores por defecto automáticamente (`CreatedAt`).

### Estructura

```
ActivityBuilder
  .WithTitle()
  .WithDescription()
  .WithDate()
  .WithCategory()
  .WithPriority()
  .Build() → Activity

TaskBuilder
  .WithActivityId()
  .WithTitle()
  .WithDescription()
  .WithDueTime()
  .Build() → Task
```

### Diagrama

```mermaid
classDiagram
    class ActivityBuilder {
        -Activity _activity
        +WithTitle(title) ActivityBuilder
        +WithDescription(desc) ActivityBuilder
        +WithDate(date) ActivityBuilder
        +WithCategory(cat) ActivityBuilder
        +WithPriority(priority) ActivityBuilder
        +Build() Activity
    }
    class TaskBuilder {
        -Task _task
        +WithActivityId(id) TaskBuilder
        +WithTitle(title) TaskBuilder
        +WithDescription(desc) TaskBuilder
        +WithDueTime(time) TaskBuilder
        +Build() Task
    }
    class ActivityController {
        +CreateActivity(dto)
    }
    class TaskController {
        +CreateTask(activityId, dto)
    }

    ActivityController --> ActivityBuilder : usa
    TaskController --> TaskBuilder : usa
    ActivityBuilder --> Activity : construye
    TaskBuilder --> Task : construye
```

### Antes vs Después

**Antes** (manual en el controlador):
```csharp
var activity = new Activity
{
    Title = dto.Title,
    Description = dto.Description,
    Date = dto.Date,
    Category = dto.Category,
    Priority = dto.Priority ?? "Normal"
};
```

**Después** (Builder):
```csharp
var activity = new ActivityBuilder()
    .WithTitle(dto.Title)
    .WithDescription(dto.Description)
    .WithDate(dto.Date)
    .WithCategory(dto.Category)
    .WithPriority(dto.Priority ?? "Normal")
    .Build();
```
