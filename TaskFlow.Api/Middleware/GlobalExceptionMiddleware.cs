using System.Net;
using System.Text.Json;

namespace TaskFlow.Api.Middleware
{
    /// <summary>
    /// Middleware para manejo centralizado de excepciones.
    /// Captura todas las excepciones no manejadas y devuelve respuestas consistentes.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse
            {
                Message = exception.Message,
                StatusCode = HttpStatusCode.InternalServerError
            };

            switch (exception)
            {
                case KeyNotFoundException:
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "El recurso solicitado no fue encontrado.";
                    break;

                case ArgumentException:
                case ArgumentNullException:
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Los parámetros proporcionados son inválidos.";
                    break;

                case InvalidOperationException:
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Operación inválida.";
                    break;

                default:
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Ocurrió un error interno del servidor.";
                    break;
            }

            context.Response.StatusCode = (int)response.StatusCode;
            return context.Response.WriteAsJsonAsync(response);
        }
    }

    /// <summary>
    /// Modelo de respuesta de error estandarizado.
    /// </summary>
    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public HttpStatusCode StatusCode { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
