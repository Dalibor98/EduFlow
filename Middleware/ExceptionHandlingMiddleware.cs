using EduFlow.Models;
using System.Text.Json;

namespace EduFlow.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
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
