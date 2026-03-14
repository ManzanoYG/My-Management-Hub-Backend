using System.Diagnostics;

namespace MyManagementHub_API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private const int SlowRequestThresholdMs = 1000;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestId = Activity.Current?.Id ?? context.TraceIdentifier;
            var method = context.Request.Method;
            var path = context.Request.Path;
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            _logger.LogInformation(
                "HTTP {Method} {Path} started | IP: {IpAddress} | RequestId: {RequestId}",
                method, path, ip, requestId);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var elapsed = stopwatch.ElapsedMilliseconds;
                var status = context.Response.StatusCode;

                _logger.LogInformation(
                    "HTTP {Method} {Path} → {StatusCode} in {ElapsedMs}ms | RequestId: {RequestId}",
                    method, path, status, elapsed, requestId);

                if (elapsed > SlowRequestThresholdMs)
                {
                    _logger.LogWarning(
                        "SLOW REQUEST: {Method} {Path} took {ElapsedMs}ms (threshold: {ThresholdMs}ms) | RequestId: {RequestId}",
                        method, path, elapsed, SlowRequestThresholdMs, requestId);
                }
            }
        }
    }
}
