namespace Infrastructure.Ef.DbEntities
{
    public class DbAuditLog
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? IpAddress { get; set; }
    }
}
