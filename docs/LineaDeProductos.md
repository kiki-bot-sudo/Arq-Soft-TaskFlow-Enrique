# Línea de productos de software: TaskFlow

## Definición

Una línea de productos de software es una familia de aplicaciones que comparte una base común y varía algunas características para responder a diferentes usuarios.

## Producto base

El producto base es **TaskFlow Agenda Personal**: una web con CRUD de tareas, descripción, fecha límite, prioridad y estado.

## Características comunes

- Crear, consultar, editar y eliminar tareas.
- Marcar tareas como pendientes o completadas.
- Persistencia de datos.
- Validación de título.
- Interfaz web responsive.
- Registro e inicio de sesión.
- Tareas privadas por usuario.

## Características variables

- Categorías académicas.
- Usuarios y organizaciones.
- Colaboración.
- Recordatorios.
- Integración con calendarios.
- Recomendaciones con inteligencia artificial.
- Almacenamiento local o en la nube.

## Variantes

- **Agenda personal básica:** funciones actuales.
- **Agenda académica:** materias, entregas y calendario escolar.
- **Agenda empresarial:** usuarios, equipos y permisos.
- **Agenda colaborativa:** tareas compartidas y comentarios.
- **Agenda con recordatorios:** correo o notificaciones.
- **Agenda con IA:** sugerencias de prioridad y planificación.

## Modelo textual de características

```text
Agenda
├── Gestión de tareas [obligatoria]
│   ├── Crear [obligatoria]
│   ├── Consultar [obligatoria]
│   ├── Editar [obligatoria]
│   ├── Eliminar [obligatoria]
│   ├── Estado [obligatoria]
│   ├── Fecha límite [obligatoria en TaskFlow]
│   └── Prioridad [obligatoria en TaskFlow]
├── Búsqueda y filtros [opcional]
├── Estadísticas [opcional]
├── Subtareas [opcional]
├── Autenticación [obligatoria en TaskFlow]
├── Categorías académicas [opcional]
├── Recordatorios [opcional]
├── Colaboración [opcional]
├── Inteligencia artificial [opcional]
└── Almacenamiento [alternativa]
    ├── Base de datos local
    └── Base de datos en la nube
```

## Beneficios de reutilización

Se reutilizarían las entidades, validaciones, CRUD, interfaz base y pruebas. Cada variante agregaría únicamente módulos o campos propios.

## Riesgos y dificultades

- Demasiadas variantes pueden complicar el código.
- Autenticación y colaboración requieren reglas nuevas.
- Recordatorios dependen de servicios externos.
- Es necesario distinguir claramente características comunes y opcionales.

## Conclusión

TaskFlow puede ser el núcleo de una familia de agendas. Por ahora solo se implementa la variante personal; las demás son una propuesta académica y no componentes existentes.
