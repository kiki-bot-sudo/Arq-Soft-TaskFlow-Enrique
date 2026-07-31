# ADR 005: Microsoft Azure

**Estado:** Aceptado
**Fecha:** 2026-07-29

## Contexto
La agenda debe publicarse en Internet usando servicios compatibles con .NET y SQL Server.

## Decisión
Desplegar la aplicación en Azure App Service y los datos en Azure SQL Database.

## Alternativas consideradas
Servidor propio y otros proveedores de nube.

## Consecuencias

### Positivas
Integración con .NET, HTTPS administrado, configuración por variables y monitoreo disponible.

### Negativas
Puede generar costo, requiere configurar recursos y depende de la disponibilidad de Azure.
