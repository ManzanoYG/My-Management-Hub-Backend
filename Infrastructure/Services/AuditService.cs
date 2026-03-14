using Infrastructure.Ef.AuditLog;
using Infrastructure.Ef.DbEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditLogRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditService> _logger;

        public AuditService(
            IAuditLogRepository repository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuditService> logger)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public void Log(string? username, string action, string entity)
        {
            try
            {
                var log = new DbAuditLog
                {
                    Username = username,
                    Action = action,
                    Entity = entity,
                    CreatedAt = DateTime.UtcNow,
                    IpAddress = ResolveIpAddress()
                };

                _repository.Add(log);
            }
            catch (Exception ex)
            {
                // Audit failure must never break the main operation.
                _logger.LogError(ex,
                    "Failed to write audit log. Username: {Username}, Action: {Action}, Entity: {Entity}",
                    username, action, entity);
            }
        }

        private string? ResolveIpAddress()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return null;

            // Respect reverse-proxy forwarded header.
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwardedFor))
            {
                return forwardedFor.Split(',').First().Trim();
            }

            return context.Connection.RemoteIpAddress?.ToString();
        }
    }
}
