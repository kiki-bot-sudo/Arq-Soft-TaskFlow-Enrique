# ADR 002: SQL Server

**Estado:** Aceptado

## Contexto
Las tareas deben conservarse y mantener relaciones e integridad.

## Decisión
Usar SQL Server en desarrollo y Azure SQL Database en la nube.

## Alternativas consideradas
SQLite y PostgreSQL.

## Consecuencias positivas
Integración directa con .NET, herramientas conocidas y transición sencilla a Azure SQL.

## Consecuencias negativas
SQL Server local requiere instalación y Azure SQL genera costo fuera de los niveles gratuitos.
