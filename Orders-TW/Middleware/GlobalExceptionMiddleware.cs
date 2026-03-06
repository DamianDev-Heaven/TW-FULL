using System.Net;
using System.Text.Json;

namespace Orders_TW.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionMiddleware(
            RequestDelegate next, 
            ILogger<GlobalExceptionMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
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

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = exception switch
            {
                KeyNotFoundException => HttpStatusCode.NotFound,
                ArgumentException => HttpStatusCode.BadRequest,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                InvalidOperationException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };

            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)statusCode;

            var errorResponse = new ErrorResponse
            {
                StatusCode = (int)statusCode,
                Message = exception.Message,
                Timestamp = DateTime.UtcNow
            };

            if (_environment.IsDevelopment())
            {
                errorResponse.Details = exception.StackTrace;
                errorResponse.Type = exception.GetType().Name;
            }

            if (statusCode == HttpStatusCode.InternalServerError)
            {
                _logger.LogError(exception, 
                    "Error interno del servidor. Path: {Path}, Method: {Method}", 
                    context.Request.Path, 
                    context.Request.Method);
            }
            else if (statusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Recurso no encontrado: {Message}. Path: {Path}", 
                    exception.Message, 
                    context.Request.Path);
            }
            else
            {
                _logger.LogWarning("Error de validación: {Message}. Path: {Path}", 
                    exception.Message, 
                    context.Request.Path);
            }

            var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(jsonResponse);
        }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? Details { get; set; }
        public string? Type { get; set; }
    }
}