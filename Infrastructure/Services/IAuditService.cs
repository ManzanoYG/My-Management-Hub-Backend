namespace Infrastructure.Services
{
    public interface IAuditService
    {
        void Log(string? username, string action, string entity);
    }
}
