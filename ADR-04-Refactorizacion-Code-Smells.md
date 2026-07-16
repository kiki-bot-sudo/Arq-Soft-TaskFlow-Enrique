# ADR-04: Refactorización de Code Smells en TaskFlow

| Campo     | Valor                    |
|-----------|--------------------------|
| Autor     | Enrique Zavala           |
| Fecha     | 16/07/2026               |
| Estado    | Aceptado                 |
| Basado en | ADR-01, ADR-02, ADR-03   |
| Revisión  | 1.0                      |

---

## Contexto

Como parte de una revisión de calidad de código sobre la rama `diagramas` (rama con los cambios más recientes del proyecto), se auditó el código en busca de *code smells*: señales de diseño que no rompen la funcionalidad pero dificultan el mantenimiento, la extensibilidad o introducen riesgos de seguridad.

Se revisaron los controladores, servicios, repositorios, DTOs, modelos de dominio y la configuración de arranque (`Program.cs`). Se identificaron 5 smells concretos, de los cuales se corrigieron 4 directamente en código; el quinto se documenta como riesgo a monitorear porque su corrección requeriría cambios de infraestructura fuera del alcance de esta revisión.

---

## Decisión

Se corrigen los siguientes code smells:

### 1. Strings mágicos duplicados para `Priority`

**Antes:** el valor `"Low"/"Normal"/"High"` estaba repetido de forma independiente en `Activity.cs` (valor por defecto), `CreateActivityDto.cs` y `UpdateActivityDto.cs` (expresión regular de validación), y `PriorityDescSortStrategy.cs` (diccionario de orden). No existía una única fuente de verdad, por lo que agregar un nuevo nivel de prioridad exigiría tocar cuatro archivos y era fácil olvidar alguno.

**Corrección:** se creó `TaskFlow.Domain.Models.PriorityLevels`, una clase estática con las constantes `Low`, `Normal`, `High`, el patrón de validación (`ValidationPattern`) y el diccionario de orden (`SortOrder`). Todos los puntos anteriores ahora referencian esta única fuente.

**Nota de diseño:** se optó por constantes de string en vez de un `enum` de C# para no requerir una nueva migración de Entity Framework (la columna `Priority` ya existe como `nvarchar(10)` en la base de datos con datos migrados).

### 2. Campos muertos e inconsistencia de estilo en `UpdateTaskDto`

**Antes:** `UpdateTaskDto` incluía las propiedades `Id` y `ActivityId`, pero `TaskController.UpdateTask` nunca las lee — usa los valores de la ruta (`{id}` y `{activityId}`). Además, era el único DTO del proyecto que decoraba sus propiedades con `[JsonPropertyName(...)]` explícitos, mientras el resto de los DTOs confía en el naming por defecto de `System.Text.Json` (camelCase automático). Esto podía confundir a quien consumiera la API, sugiriendo que enviar `id`/`activityId` en el body tenía efecto.

**Corrección:** se eliminaron `Id` y `ActivityId` de `UpdateTaskDto` y los atributos `JsonPropertyName` redundantes, dejando el estilo consistente con `CreateTaskDto`, `CreateActivityDto` y `UpdateActivityDto`. Se agregó un comentario XML explicando por qué esos identificadores no están en el DTO.

### 3. Comentario engañoso sobre borrado en cascada

**Antes:** el XML-doc de `ActivityController.DeleteActivity` afirmaba "Elimina una actividad **y sus tareas asociadas**", dando la impresión de que el controlador o el servicio contienen lógica explícita para borrar las tareas. En realidad esto ocurre únicamente por la configuración `OnDelete(DeleteBehavior.Cascade)` en `TaskFlowDbContext`. Si alguien cambiara esa configuración sin notar el comentario del controller, el comportamiento documentado dejaría de cumplirse silenciosamente.

**Corrección:** se actualizó el comentario para que apunte explícitamente a la configuración real (`TaskFlowDbContext`, `DeleteBehavior.Cascade`), dejando trazabilidad de dónde vive el comportamiento.

### 4. Política de CORS permisiva sin distinción de entorno

**Antes:** `Program.cs` registraba una única política `AllowAll` con `AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()`, aplicada igual en desarrollo y en producción. Una política de CORS totalmente abierta en producción permite que cualquier sitio web haga peticiones autenticadas a la API desde el navegador del usuario, ampliando la superficie de ataque (p. ej. CSRF-like abuse, exfiltración de datos si en el futuro se agrega autenticación).

**Corrección:** la política ahora depende del entorno (`builder.Environment.IsDevelopment()`):
- En **Development** se mantiene el comportamiento abierto anterior, para no complicar las pruebas locales.
- En **otros entornos**, los orígenes permitidos se leen de la nueva sección de configuración `AllowedCorsOrigins` en `appsettings.json` (vacía por defecto, a completar por el equipo de despliegue con los dominios reales del frontend).

### ¿Por qué?

Estos cuatro cambios reducen deuda técnica sin alterar el comportamiento observable de la API en desarrollo: las pruebas existentes (`BuilderTests`, `ActivityServiceTests`) siguen pasando porque los valores de `PriorityLevels` son idénticos a los strings que reemplazan, y ningún cliente de la API dependía de enviar `id`/`activityId` en el body de `PUT /api/activity/{activityId}/task/{id}` (esos valores ya se ignoraban).

### Alternativas consideradas

| Alternativa | Por qué la descarté |
|-------------|---------------------|
| Convertir `Priority` a `enum` de C#/EF Core | Requiere una nueva migración de base de datos y coordinarla con datos ya sembrados (`HasData`); fuera del alcance de una limpieza de code smells que no debe tocar el esquema. |
| Dejar `AllowAnyOrigin` en todos los entornos | Es la causa raíz de un riesgo de seguridad real en producción; documentarlo sin corregirlo hubiera dejado el smell más importante sin resolver. |
| Bloquear CORS por completo en producción (sin lista configurable) | Rompería cualquier frontend legítimo sin dar una vía de configuración; una lista vacía por defecto obliga a decidirlo explícitamente al desplegar, sin adivinar dominios que no conozco. |

---

## Smell identificado y **no** corregido en este ADR

**Cadena de conexión sin parametrizar por entorno:** `appsettings.json` y `appsettings.Development.json` contienen una `DefaultConnection` idéntica y hardcodeada (`Server=localhost\SQLEXPRESS`, `Trusted_Connection=True`). No es una fuga de credenciales (usa autenticación integrada de Windows, no usuario/contraseña), pero acopla el proyecto a una instancia local específica y duplica configuración entre archivos que deberían diferir por entorno. Se documenta aquí en lugar de corregirse porque la solución correcta (variables de entorno o *user secrets* por entorno de despliegue) depende de decisiones de infraestructura del equipo que exceden una limpieza de código.

---

## Consecuencias

- ✅ Un solo lugar (`PriorityLevels`) para agregar o cambiar niveles de prioridad.
- ✅ `UpdateTaskDto` refleja fielmente lo que la API realmente usa.
- ✅ El comentario de `DeleteActivity` ya no puede desincronizarse silenciosamente del código real.
- ✅ CORS deja de ser una superficie de ataque abierta por defecto en producción.
- ⚠️ Antes de desplegar a producción, el equipo debe completar `AllowedCorsOrigins` en `appsettings.json` (o vía variables de entorno) con los dominios reales del frontend; si se deja vacío, ningún origen podrá llamar a la API fuera de Development.
- ⚠️ La cadena de conexión sigue pendiente de parametrizar (ver sección anterior).
