using System.Net;

namespace MyManagementHub_API.Middleware
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
            var requestId = context.TraceIdentifier;

            var (statusCode, message) = exception switch
            {
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized."),
                KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found."),
                ArgumentException => (HttpStatusCode.BadRequest, exception.Message),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
            };

            // SQL / EF errors are logged as critical for easy filtering in file.
            if (statusCode == HttpStatusCode.InternalServerError)
            {
                _logger.LogCritical(exception,
                    "Unhandled exception | Method: {Method} | Path: {Path} | RequestId: {RequestId}",
                    context.Request.Method, context.Request.Path, requestId);
            }
            else
            {
                _logger.LogWarning(exception,
                    "Handled exception ({ExceptionType}) | Method: {Method} | Path: {Path} | RequestId: {RequestId}",
                    exception.GetType().Name, context.Request.Method, context.Request.Path, requestId);
            }

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                Message = message,
                RequestId = requestId
            });
        }
    }
}
