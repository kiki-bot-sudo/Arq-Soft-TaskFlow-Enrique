# ADR 004: Separación por capas

**Estado:** Aceptado
**Fecha:** 2026-07-29

## Contexto
El código debe ser sencillo de explicar y separar la web, las reglas y la persistencia.

## Decisión
Conservar cuatro capas: Domain, Application, Infrastructure y API, más un proyecto de pruebas.

## Alternativas consideradas
Un único proyecto y microservicios.

## Consecuencias

### Positivas
Responsabilidades claras, pruebas más sencillas y cambios de interfaz o datos localizados.

### Negativas
Hay más proyectos y referencias que en una aplicación monolítica de un solo archivo.
