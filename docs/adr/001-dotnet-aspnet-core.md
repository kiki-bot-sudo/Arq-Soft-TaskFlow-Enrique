# ADR 001: .NET y ASP.NET Core

**Estado:** Aceptado
**Fecha:** 2026-07-29

## Contexto
El proyecto necesita una plataforma conocida en clase, multiplataforma y apta para una API web.

## Decisión
Usar .NET 8 y ASP.NET Core Web API.

## Alternativas consideradas
Node.js con Express y Java con Spring Boot.

## Consecuencias

### Positivas
Tipado fuerte, buen soporte académico, inyección de dependencias integrada y despliegue directo en Azure.

### Negativas
Requiere instalar el SDK y conocer C#; el hosting necesita un runtime compatible.
