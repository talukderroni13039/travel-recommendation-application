using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TravelRecommendation.Application.Middleware
{
    // Middleware/GlobalExceptionHandlerMiddleware.cs
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler( RequestDelegate next,   ILogger<GlobalExceptionHandler> logger)
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
                // Just log the exception
                _logger.LogError(ex,
                    "Exception occurred: {ExceptionType} in {ClassName}.{MethodName} - {Message}",
                    ex.GetType().Name,
                    ex.TargetSite?.DeclaringType?.Name ?? "Unknown",
                    ex.TargetSite?.Name ?? "Unknown",
                    ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static  Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Determine status code and message based on exception type
            var (statusCode, message) = exception switch
            {
                ArgumentException => (400, exception.Message),
                InvalidOperationException => (400, exception.Message),
                KeyNotFoundException => (404, "Requested resource not found"),
                HttpRequestException => (503, "External service unavailable"),
                _ => (500, "An unexpected error occurred")
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new
            {
                error = message,
                statusCode,
                timestamp = DateTime.UtcNow
            };
            var jsonResponse = JsonConvert.SerializeObject(response);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
