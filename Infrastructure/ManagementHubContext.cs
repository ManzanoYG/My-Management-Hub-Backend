using Infrastructure.Ef.DbEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class ManagementHubContext : DbContext
    {
        public ManagementHubContext(DbContextOptions options) : base(options) { }

        public DbSet<DbUser> Users { get; set; }
        public DbSet<DbAuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbUser>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(u => u.Username);
                entity.Property(u => u.Username).HasColumnName("username");
                entity.Property(u => u.Password).HasColumnName("password");
                entity.Property(u => u.Created_at).HasColumnName("created_at");
                entity.Property(u => u.Updated_at).HasColumnName("updated_at");
                entity.Property(u => u.IsBanned).HasColumnName("isBanned");
                entity.Property(u => u.UserType).HasColumnName("userType");
            });

            modelBuilder.Entity<DbAuditLog>(entity =>
            {
                entity.ToTable("audit_logs");
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Id).HasColumnName("Id").ValueGeneratedOnAdd();
                entity.Property(a => a.Username).HasColumnName("Username").HasMaxLength(255).IsRequired(false);
                entity.Property(a => a.Action).HasColumnName("Action").HasMaxLength(200).IsRequired();
                entity.Property(a => a.Entity).HasColumnName("Entity").HasMaxLength(100).IsRequired();
                entity.Property(a => a.CreatedAt).HasColumnName("CreatedAt").IsRequired();
                entity.Property(a => a.IpAddress).HasColumnName("IpAddress").HasMaxLength(50).IsRequired(false);
            });
        }
    }
}
