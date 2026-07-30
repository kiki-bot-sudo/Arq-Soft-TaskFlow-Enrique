# ADR 007: Autenticación con ASP.NET Core Identity

**Estado:** Aceptado

## Contexto

TaskFlow necesita registro, inicio de sesión y separación de tareas por usuario sin incorporar un servidor de identidad externo.

## Decisión

Usar ASP.NET Core Identity con cookies HTTP y las tablas Identity almacenadas en la misma base SQL Server. El servidor obtiene el usuario desde la cookie y nunca acepta un `UserId` enviado por el frontend.

## Alternativas consideradas

- JWT: útil para clientes externos, pero agrega manejo manual de tokens que esta web no necesita.
- Sesiones y contraseñas propias: más simples en apariencia, pero menos seguras.
- Microsoft Entra ID: adecuado para organizaciones, pero excesivo para el alcance escolar.

## Consecuencias positivas

- Contraseñas cifradas mediante hashing seguro de Identity.
- Cookies `HttpOnly`.
- Validación de correo único y políticas de contraseña.
- Herramientas integradas con EF Core.

## Consecuencias negativas

- Se agregan varias tablas Identity.
- La aplicación depende de cookies y del mismo dominio para frontend y API.
- Recuperación de contraseña y verificación por correo quedan fuera de esta versión.
