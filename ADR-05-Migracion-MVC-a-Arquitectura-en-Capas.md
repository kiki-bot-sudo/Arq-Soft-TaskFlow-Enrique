# ADR-05: Migración de MVC a Arquitectura en Capas

| Campo     | Valor                              |
|-----------|-------------------------------------|
| Autor     | Enrique Zavala                     |
| Fecha     | 16/07/2026                         |
| Estado    | Aceptado                           |
| Basado en | ADR-01, ADR-02, ADR-03             |
| Reemplaza | Parcialmente a ADR-02               |
| Revisión  | 1.0                                 |

---

## Contexto

ADR-02 documentó una arquitectura inicial para TaskFlow que combinaba **Controllers MVC** (para vistas web) junto con **Controllers de API REST**, ambos apoyados en una única capa de `Services` y `Repositories`. Esa versión también contemplaba PostgreSQL y AutoMapper para el mapeo DTO ↔ entidad.

Al continuar el desarrollo, se decidió **eliminar por completo la capa MVC** (controllers y vistas) y migrar el proyecto a una **arquitectura en capas separadas por proyecto** (`Domain`, `Application`, `Infrastructure`, `Api`, `Tests`), dejando la API REST como único punto de entrada. Esta decisión ya se refleja en el código actual y fue documentada parcialmente en ADR-03 (que describe la arquitectura de 5 capas resultante), pero nunca se dejó constancia explícita de **por qué se abandonó MVC**, que es lo que este ADR cubre.

---

## Decisión

Se elimina la capa de Controllers MVC (vistas Razor y controladores orientados a renderizar HTML) y el proyecto queda estructurado únicamente como:

- **TaskFlow.Domain** — entidades y Builders, sin dependencias de frameworks web.
- **TaskFlow.Application** — servicios, interfaces, Observer, Decorator, Strategy.
- **TaskFlow.Infrastructure** — DbContext, repositorios, migraciones.
- **TaskFlow.Api** — único punto de entrada HTTP, expuesto exclusivamente como API REST (sin vistas).
- **TaskFlow.Tests** — pruebas unitarias sobre Application y Domain.

El consumo de la aplicación (web, móvil, etc.) queda del lado de un cliente externo que consume la API, en vez de que el propio backend renderice HTML.

### ¿Por qué?

- **MVC no escala bien a medida que el proyecto crece.** Mezclar la lógica de presentación (vistas, `ViewModels`) con los mismos controllers que ya exponían la API generaba duplicación y controllers con demasiadas responsabilidades.
- **Separar en capas por proyecto hace las responsabilidades más explícitas**: el compilador impide que `Domain` dependa de `Infrastructure` o de ASP.NET Core, por ejemplo, algo que con MVC en un solo proyecto no se garantiza — es fácil terminar llamando al `DbContext` directamente desde un controller de vista.
- **Es más fácil de testear**: `TaskFlow.Application` y `TaskFlow.Domain` no dependen de ASP.NET Core, por lo que las pruebas unitarias (`BuilderTests`, `ActivityServiceTests`, etc.) no necesitan levantar un host web ni mockear el pipeline de MVC.
- **Encaja mejor con los patrones GoF de ADR-03** (Builder, Observer, Decorator, Strategy): al no haber vistas ni `ViewModels` de por medio, los patrones operan directamente sobre las entidades de dominio y los DTOs de la API, sin una tercera representación (los `ViewModels` de MVC) que mantener sincronizada.

### Alternativas consideradas

| Alternativa | Por qué la descarté |
|-------------|---------------------|
| Mantener MVC y API REST en paralelo (como en ADR-02) | Duplicaba lógica entre `TaskController` (MVC) y `TaskApiController` (REST) que terminaban llamando a los mismos servicios; mantenerlos sincronizados agregaba trabajo sin aportar valor real al proyecto. |
| Migrar a Blazor Server en vez de eliminar la vista | Habría resuelto la duplicación de controllers, pero seguía acoplando la interfaz de usuario al mismo backend, contradiciendo el objetivo de que la API sea consumible por múltiples clientes (web, móvil) de forma independiente. |
| Dejar el proyecto en una sola capa (`TaskFlow.Api` con todo adentro) | Resolvía el problema de MVC, pero no resolvía el problema de fondo: sin separación por proyecto, nada impide que el código de presentación vuelva a mezclarse con el de acceso a datos según crezca el equipo o el alcance. |

---

## Consecuencias

**✅ Lo que gano:**

- **Técnica:** cada capa se puede compilar, testear y razonar de forma aislada; `Domain` y `Application` no dependen de ASP.NET Core, lo que los hace reutilizables si en el futuro se agrega otro tipo de cliente (por ejemplo, un worker de notificaciones).
- **Proceso:** al no haber vistas que mantener, cada cambio de API solo se define una vez (DTOs + Controller), en vez de coordinarlo entre un controller de vista y uno de API como pasaba con el diseño de ADR-02.

**⚠️ Lo que sacrifico o asumo:**

- **Técnica:** ya no hay una interfaz web propia; cualquier cliente (incluyendo un frontend web) debe construirse por separado y consumir la API vía HTTP, lo cual agrega un proyecto/despliegue adicional si se quiere una UI.
- **Deuda/riesgo:** ADR-02 queda parcialmente desactualizado — describe MVC, PostgreSQL y AutoMapper, ninguno de los cuales refleja el estado actual del código (que usa SQL Server sin MVC ni AutoMapper). Se recomienda actualizar o marcar como superado el contenido de ADR-02 relativo a MVC para que no confunda a quien lo lea después de este ADR.
