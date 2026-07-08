# Modelo C4 — TaskFlow

Documentación de la arquitectura de **TaskFlow** (API RESTful de gestión de actividades
y tareas personales, ASP.NET Core 8 + arquitectura en capas + patrones GoF) usando el
modelo C4, versionada como código (Mermaid) dentro del repositorio.

---

## Nivel 1 — Contexto

**Para quién es:** stakeholders no técnicos (profesor, cliente, nuevo integrante del equipo).
**Pregunta que responde:** ¿qué es el sistema y quién lo usa? Sin entrar en tecnología ni
en piezas internas.

```mermaid
C4Context
    title Diagrama de Contexto — TaskFlow

    Person(usuario, "Usuario", "Persona que organiza su día a día: crea actividades y las tareas necesarias para completarlas.")

    System(taskflow, "TaskFlow", "Sistema de gestión de actividades y tareas personales. Permite crear, organizar, ordenar y dar seguimiento al progreso de las actividades del día.")

    System_Ext(cliente_http, "Cliente HTTP", "Swagger UI / Postman / futura app web o móvil que consume la API de TaskFlow.")

    Rel(usuario, cliente_http, "Interactúa con", "Navegador / App")
    Rel(cliente_http, taskflow, "Crea, consulta, actualiza y elimina actividades y tareas", "HTTPS/JSON")
```

### Notas del Nivel 1

- El **usuario** nunca habla directamente con el sistema por fuera de un cliente HTTP:
  hoy ese cliente es Swagger UI (incluido en el propio API), pero el diseño no acopla
  el sistema a ningún cliente específico — cualquier app web o móvil podría consumirlo.
- TaskFlow **no depende de sistemas externos** de terceros (no hay integración con
  correo, pagos, calendario externo, etc. en el estado actual del proyecto).
- La base de datos **no aparece en este nivel** a propósito: en Contexto solo interesa
  el sistema como una caja única frente al usuario; el detalle de sus piezas internas
  (incluida la base de datos) se muestra en el Nivel 2.

---

## Nivel 2 — Contenedores

**Para quién es:** el equipo técnico y quien vaya a desplegar o integrar el sistema.
**Pregunta que responde:** ¿cuáles son las piezas grandes que se ejecutan por separado
(aplicaciones, bases de datos) y cómo se comunican entre sí?

```mermaid
C4Container
    title Diagrama de Contenedores — TaskFlow

    Person(usuario, "Usuario", "Persona que organiza su día a día.")

    System_Boundary(taskflow, "TaskFlow") {
        Container(api, "TaskFlow.Api", "ASP.NET Core 8 Web API", "Expone los endpoints REST de Actividades y Tareas. Incluye Swagger UI, middleware de manejo global de excepciones y la orquestación de los patrones GoF aplicados (Builder, Strategy, Decorator, Observer).")
        ContainerDb(db, "Base de Datos TaskFlowDb", "SQL Server", "Almacena Activities y Tasks. Accedida vía Entity Framework Core (migraciones y seed data incluidos).")
    }

    Rel(usuario, api, "Crea, consulta, actualiza y elimina actividades/tareas", "HTTPS/JSON (REST), vía Swagger UI u otro cliente")
    Rel(api, db, "Lee y escribe", "EF Core / TCP-SQL")
```

### Notas del Nivel 2

- Aunque internamente `TaskFlow.Api` se compone de varias capas (`Application`, `Domain`,
  `Infrastructure`), técnicamente es **un solo contenedor**: se compila y despliega como
  un único proceso ASP.NET Core. Esas capas son componentes internos, no contenedores
  independientes — se detallan en el Nivel 3.
- El único contenedor con estado persistente es la base de datos `TaskFlowDb` (SQL Server),
  gestionada con migraciones de EF Core (`TaskFlow.Infrastructure/Migrations`).
- No existe todavía un contenedor de frontend (SPA/móvil) en el repositorio; el consumo
  se hace hoy vía Swagger UI, servido dentro del mismo contenedor del API.

---

## Nivel 3 — Componentes

**Para quién es:** desarrolladores que van a trabajar dentro del contenedor `TaskFlow.Api`.
**Pregunta que responde:** ¿qué piezas hay dentro del contenedor principal, cómo se
comunican, y qué patrones GoF ya están implementados?

```mermaid
C4Component
    title Diagrama de Componentes — dentro de TaskFlow.Api

    Person(usuario, "Usuario", "Vía HTTP/Swagger")

    Container_Boundary(api, "TaskFlow.Api") {
        Component(activityCtrl, "ActivityController", "ASP.NET Controller", "Endpoints REST de actividades. Usa ActivityBuilder para construir la entidad desde el DTO.")
        Component(taskCtrl, "TaskController", "ASP.NET Controller", "Endpoints REST de tareas. Usa TaskBuilder para construir la entidad desde el DTO.")
        Component(exMiddleware, "GlobalExceptionMiddleware", "Middleware", "Captura excepciones no controladas y responde con un error HTTP consistente.")

        Component(builders, "ActivityBuilder / TaskBuilder", "GoF Builder", "Construyen instancias de Activity/Task paso a paso a partir de los DTOs de entrada.")

        Component(decorator, "LoggingActivityServiceDecorator", "GoF Decorator", "Envuelve a ActivityService agregando logging antes/después de cada operación, sin modificarlo.")
        Component(activityService, "ActivityService", "Application Service", "Lógica de negocio de actividades. Usa una IActivitySortStrategy para ordenar resultados.")
        Component(strategy, "IActivitySortStrategy\n(PriorityDescSortStrategy / DateAscSortStrategy)", "GoF Strategy", "Algoritmos intercambiables de ordenamiento de actividades.")

        Component(taskService, "TaskService", "Application Service", "Lógica de negocio de tareas. Actúa como Sujeto del patrón Observer.")
        Component(observer, "ActivityCompletionObserver", "GoF Observer", "Al actualizarse una tarea, revisa si todas las tareas de la actividad están completas y auto-completa/reabre la actividad.")

        Component(activityRepo, "ActivityRepository", "Infrastructure Repository", "Acceso a datos de Activity vía EF Core.")
        Component(taskRepo, "TaskRepository", "Infrastructure Repository", "Acceso a datos de Task vía EF Core.")
        Component(dbContext, "TaskFlowDbContext", "EF Core DbContext", "Mapeo objeto-relacional de Activity y Task.")
    }

    ContainerDb(db, "TaskFlowDb", "SQL Server", "Base de datos relacional")

    Rel(usuario, activityCtrl, "HTTP", "REST/JSON")
    Rel(usuario, taskCtrl, "HTTP", "REST/JSON")
    Rel(activityCtrl, builders, "Construye entidad con")
    Rel(taskCtrl, builders, "Construye entidad con")
    Rel(activityCtrl, decorator, "Llama a IActivityService (resuelto como)")
    Rel(decorator, activityService, "Delega en (envuelve)")
    Rel(activityService, strategy, "Ordena resultados con")
    Rel(taskCtrl, taskService, "Llama a ITaskService")
    Rel(taskService, observer, "Notifica al actualizar una tarea")
    Rel(observer, taskRepo, "Consulta tareas de la actividad")
    Rel(activityService, activityRepo, "Usa")
    Rel(taskService, taskRepo, "Usa")
    Rel(observer, activityRepo, "Auto-completa/reabre actividad")
    Rel(activityRepo, dbContext, "Usa")
    Rel(taskRepo, dbContext, "Usa")
    Rel(dbContext, db, "EF Core", "SQL")
```

### Notas del Nivel 3

- Los **4 patrones GoF** implementados en el proyecto (documentados también en
  `PATRONES-GOF.md` y `ADR-03`) quedan explícitos como componentes:
  - **Builder** (`ActivityBuilder`, `TaskBuilder`) — construcción de entidades desde DTOs.
  - **Strategy** (`IActivitySortStrategy`) — algoritmo de orden intercambiable en tiempo de ejecución.
  - **Decorator** (`LoggingActivityServiceDecorator`) — logging transversal sin tocar `ActivityService`.
  - **Observer** (`ActivityCompletionObserver`) — auto-completado de actividades reaccionando a cambios en sus tareas.
- `GlobalExceptionMiddleware` es transversal a todos los controllers (no se dibujó cada
  relación individual para no saturar el diagrama).
- Las flechas de `Controller → Service` respetan la inyección de dependencias real
  configurada en `Program.cs`: `ActivityController` recibe `IActivityService`, cuya
  implementación registrada en el contenedor DI es el `Decorator`, que a su vez envuelve
  al `ActivityService` real.
