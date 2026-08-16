using back_end.DTOs;
using back_end.Records;
using System.Text.Json;

namespace back_end.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = exception switch
            {
                ArgumentException => StatusCodes.Status400BadRequest,

                KeyNotFoundException => StatusCodes.Status404NotFound,

                _ => StatusCodes.Status500InternalServerError
            };

            MessageCode messageCode = statusCode switch
            {
                StatusCodes.Status400BadRequest =>
                    MessageCode.RequestInvalid,

                StatusCodes.Status404NotFound =>
                    MessageCode.RequestNotFound,

                _ =>
                    MessageCode.InternalServerError
            };

            var response = ApiResponse<object?>.Response(messageCode);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred. Request: {Method} {Path}");
                await HandleExceptionAsync(context, ex);
            }
        }
    }
}
