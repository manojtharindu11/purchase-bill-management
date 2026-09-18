using System.Data.Common;
using Microsoft.Data.SqlClient;
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
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database request failed. TraceId={TraceId} Path={Path}", traceId, context.Request.Path);
                await WriteErrorAsync(
                    context,
                    GetDatabaseStatusCode(ex),
                    GetDatabaseMessage(ex),
                    traceId);
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "Database provider request failed. TraceId={TraceId} Path={Path}", traceId, context.Request.Path);
                await WriteErrorAsync(
                    context,
                    StatusCodes.Status503ServiceUnavailable,
                    "The database provider could not complete the request. Check the API database connection and migrations.",
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

        private static int GetDatabaseStatusCode(SqlException exception)
            => exception.Number == 208
                ? StatusCodes.Status500InternalServerError
                : StatusCodes.Status503ServiceUnavailable;

        private static string GetDatabaseMessage(SqlException exception)
            => exception.Number switch
            {
                208 => "The database schema is missing a required table. Run purchase_bill_management.sql against PurchaseBillManagement.",
                4060 => "The API reached SQL Server, but the PurchaseBillManagement database could not be opened. Check the database name and user permissions.",
                18456 => "The API reached SQL Server, but the database credentials were rejected. Check the SQL username and password.",
                53 or 11001 or -2 => "The API could not reach SQL Server. Check the Azure SQL server hostname, port 1433, and firewall rules.",
                _ => "The database request failed. Check the API Render logs using the supplied trace ID."
            };
    }
}
