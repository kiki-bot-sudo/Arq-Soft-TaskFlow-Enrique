# ADR-01: API REST para la gestión de TaskFlow

**Estado:** Aceptado
**Fecha:** 2026-06-05

## Contexto

TaskFlow necesita exponer la gestión de actividades y tareas a una interfaz web. El proyecto académico debe mantener separadas la presentación, las reglas de aplicación y el acceso a datos, y utilizar mecanismos HTTP conocidos y fáciles de comprobar.

## Decisión

Implementar una API REST con controladores de ASP.NET Core. Los controladores reciben solicitudes HTTP, validan los DTO y delegan las operaciones en los servicios de la capa de aplicación.

## Alternativas consideradas

- Razor Pages con acceso directo desde páginas: reduce archivos inicialmente, pero acopla más la presentación con el servidor.
- GraphQL: permite consultas flexibles, aunque agrega complejidad innecesaria para el CRUD del proyecto.
- Servicios SOAP: ofrecen contratos formales, pero requieren más configuración y no corresponden al estilo del frontend existente.

## Consecuencias

- La interfaz web puede consumir operaciones mediante HTTP y JSON.
- Los controladores se mantienen separados de la persistencia y las reglas de aplicación.
- Es posible documentar y probar la API con Swagger.
- Deben mantenerse contratos, códigos HTTP y validaciones coherentes entre el frontend y el backend.
