using Infrastructure.Ef.DbEntities;

namespace Infrastructure.Ef.AuditLog
{
    public interface IAuditLogRepository
    {
        void Add(DbAuditLog log);
    }
}
