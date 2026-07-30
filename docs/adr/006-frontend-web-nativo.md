# ADR 006: Frontend web nativo

**Estado:** Aceptado

## Contexto
El proyecto necesita una interfaz moderna sin aumentar innecesariamente su complejidad.

## Decisión
Usar HTML, CSS y JavaScript nativo en `TaskFlow.Api/wwwroot`, consumiendo la API con `fetch`.

## Alternativas consideradas
Razor Pages, Blazor, React y Angular.

## Consecuencias positivas
No agrega paquetes, se publica junto con la API y es fácil de demostrar.

## Consecuencias negativas
El manejo del DOM es manual y una interfaz mucho más grande podría necesitar componentes o un framework.
