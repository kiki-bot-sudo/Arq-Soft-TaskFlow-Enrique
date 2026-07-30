# Evaluación ATAM simplificada de TaskFlow

## Introducción

Este documento usa una versión educativa de ATAM para identificar cómo las decisiones arquitectónicas de TaskFlow afectan sus atributos de calidad.

## Objetivos y arquitectura

TaskFlow permite administrar tareas personales desde una web y conservarlas en una base de datos. La solución separa dominio, aplicación, infraestructura y API. El navegador consume controladores REST; los servicios aplican reglas; los repositorios usan Entity Framework Core y SQL Server.

## Interesados

- **Usuario:** necesita una agenda clara y confiable.
- **Desarrollador:** necesita código fácil de modificar y probar.
- **Profesor o evaluador:** necesita verificar requisitos y decisiones.
- **Administrador:** necesita desplegar, configurar y diagnosticar la aplicación.

## Atributos de calidad

- **Usabilidad:** interfaz responsive, estados visibles y errores comprensibles.
- **Mantenibilidad:** proyectos por responsabilidad y reglas centralizadas.
- **Seguridad:** Identity con cookies, contraseñas hasheadas y consultas aisladas por usuario.
- **Rendimiento:** consultas asíncronas, índices y respuestas pequeñas.
- **Disponibilidad:** endpoint de salud y servicios administrados de Azure.
- **Portabilidad:** .NET 8 funciona localmente y en Azure; la conexión cambia por configuración.

## Escenarios de calidad

### Crear una tarea
- **Fuente:** Usuario.
- **Estímulo:** Envía una tarea válida.
- **Entorno:** Operación normal.
- **Artefacto:** Frontend, API y base de datos.
- **Respuesta:** La tarea se valida, guarda y muestra.
- **Medida:** Respuesta HTTP 201 y aparición en la lista en menos de 2 segundos bajo carga escolar normal.

### Formulario vacío
- **Fuente:** Usuario.
- **Estímulo:** Envía un título vacío.
- **Entorno:** Operación normal.
- **Artefacto:** Formulario, DTO y servicio.
- **Respuesta:** Se rechaza sin insertar datos y se explica el error.
- **Medida:** HTTP 400 y mensaje visible.

### Base de datos no disponible
- **Fuente:** SQL Server o red.
- **Estímulo:** La conexión falla.
- **Entorno:** Incidente.
- **Artefacto:** Repositorio y middleware.
- **Respuesta:** La API no revela datos técnicos y registra el fallo.
- **Medida:** HTTP 500 controlado; `/health` indica estado no saludable cuando se agregue comprobación SQL específica.

### Solicitudes simultáneas
- **Fuente:** Varios usuarios o pestañas.
- **Estímulo:** Varias operaciones CRUD.
- **Entorno:** Carga normal.
- **Artefacto:** API y DbContext.
- **Respuesta:** Cada solicitud utiliza un DbContext independiente.
- **Medida:** Sin corrupción de datos y tiempos menores a 2 segundos para decenas de solicitudes.

### Modificar una validación
- **Fuente:** Desarrollador.
- **Estímulo:** Cambia la longitud máxima del título.
- **Entorno:** Mantenimiento.
- **Artefacto:** DTO, servicio y configuración EF.
- **Respuesta:** El cambio queda localizado y comprobado por pruebas.
- **Medida:** No requiere modificar el frontend completo ni los repositorios.

### Desplegar en Azure
- **Fuente:** Administrador.
- **Estímulo:** Publica la aplicación con una conexión de Azure SQL.
- **Entorno:** Producción.
- **Artefacto:** App Service y configuración.
- **Respuesta:** La misma compilación usa la cadena proporcionada por el entorno.
- **Medida:** No se modifica código fuente para desplegar.

## Riesgos

- La recuperación de contraseña y la confirmación real por correo no forman parte de esta versión escolar.
- Una caída de SQL Server impide operar.
- Las reglas de longitud están repetidas entre DTO, servicio y EF y deben mantenerse sincronizadas.
- El frontend nativo puede ser más difícil de extender si crece mucho.

## No riesgos

- La cantidad de tareas de una demostración escolar no exige microservicios ni caché distribuida.
- ASP.NET Core, EF Core y SQL Server tienen soporte estable.
- El frontend se publica con la API y no requiere un segundo servicio.

## Puntos sensibles

- Cadena de conexión y disponibilidad de SQL Server.
- Índices de fecha, estado y prioridad.
- Validación del título.
- Zona horaria de fecha límite.
- Configuración CORS si se separa el frontend.

## Trade-offs

- Un solo despliegue simplifica operación, pero escala frontend y API juntos.
- SQL Server facilita Azure, pero es más pesado que SQLite.
- JavaScript nativo reduce dependencias, pero ofrece menos estructura que un framework.
- Las capas agregan proyectos, pero mejoran claridad y pruebas.

## Conclusión

La arquitectura es suficiente para un proyecto escolar. Identity reduce el acceso cruzado: cambiar el identificador de una tarea ajena devuelve `404`. La dependencia de SQL Server permanece como principal punto operativo.
