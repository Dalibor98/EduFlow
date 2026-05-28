using EduFlow.Models;
using System.Text.Json;

namespace EduFlow.Middleware
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
        
        public async Task InvokeAsync (HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
               await HandleExceptionAsync(context,e);
            }
        }

        private async Task HandleExceptionAsync (HttpContext context,Exception exception)
        {
            var statusCode = exception switch
            {
                KeyNotFoundException => 404,
                UnauthorizedAccessException => 401,
                ArgumentException => 400,
                _ => 500
            };
            
            if (statusCode == 500)
            {
                _logger.LogError(exception, "Unhandled exception while processing {Path}",context.Request.Path);
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var error = new ErrorResponse
            {
                StatusCode = statusCode,
                Message = exception.Message,
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(error));
        }
    }
}
