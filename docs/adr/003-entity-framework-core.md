# ADR 003: Entity Framework Core

**Estado:** Aceptado
**Fecha:** 2026-07-29

## Contexto
Se necesita consultar y modificar SQL Server sin escribir manualmente cada sentencia SQL.

## Decisión
Usar Entity Framework Core 8 con migraciones.

## Alternativas consideradas
ADO.NET y Dapper.

## Consecuencias

### Positivas
Reduce código repetitivo, permite consultas LINQ y versiona el esquema mediante migraciones.

### Negativas
Oculta parte del SQL y exige revisar consultas e índices para evitar problemas de rendimiento.
