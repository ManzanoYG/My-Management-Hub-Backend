using Infrastructure.Ef.DbEntities;

namespace Infrastructure.Ef.AuditLog
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly ManagementHubContext _context;

        public AuditLogRepository(ManagementHubContext context)
        {
            _context = context;
        }

        public void Add(DbAuditLog log)
        {
            _context.AuditLogs.Add(log);
            _context.SaveChanges();
        }
    }
}
