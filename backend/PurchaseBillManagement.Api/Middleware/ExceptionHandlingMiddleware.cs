using System.Data.Common;
using PurchaseBillManagement.Api.Services.Exceptions;

namespace PurchaseBillManagement.Api.Middleware
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

        public async Task InvokeAsync(HttpContext context)
        {
            var traceId = context.TraceIdentifier;

            try
            {
                await _next(context);
            }
            catch (ApiException ex)
            {
                _logger.LogWarning(ex, "Request failed. TraceId={TraceId} Path={Path}", traceId, context.Request.Path);
                await WriteErrorAsync(context, ex.StatusCode, ex.Message, traceId);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized request. TraceId={TraceId} Path={Path}", traceId, context.Request.Path);
                await WriteErrorAsync(context, StatusCodes.Status401Unauthorized, ex.Message, traceId);
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "Database request failed. TraceId={TraceId} Path={Path}", traceId, context.Request.Path);
                await WriteErrorAsync(
                    context,
                    StatusCodes.Status503ServiceUnavailable,
                    "The database is unavailable or its schema is not initialized. Check the API database connection and migrations.",
                    traceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception. TraceId={TraceId} Path={Path}", traceId, context.Request.Path);
                await WriteErrorAsync(context, StatusCodes.Status500InternalServerError,
                    "An unexpected server error occurred. Check the API logs using the supplied trace ID.", traceId);
            }
        }

        private static async Task WriteErrorAsync(HttpContext context, int statusCode, string message, string traceId)
        {
            context.Response.StatusCode = statusCode;
            context.Response.Headers.Append("X-Trace-Id", traceId);
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { statusCode, message, traceId });
        }
    }
}
