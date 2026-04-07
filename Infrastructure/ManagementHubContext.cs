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
        public DbSet<DbNote> Notes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbUser>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id).HasColumnName("id").HasDefaultValueSql("NEWID()");
                entity.HasIndex(u => u.Username).IsUnique();
                entity.Property(u => u.Username).HasColumnName("username");
                entity.Property(u => u.Password).HasColumnName("password");
                entity.Property(u => u.TimeZone).HasColumnName("timeZone");
                entity.Property(u => u.Created_at).HasColumnName("created_at");
                entity.Property(u => u.Updated_at).HasColumnName("updated_at");
                entity.Property(u => u.IsBanned).HasColumnName("isBanned").HasDefaultValue(false);
                entity.Property(u => u.UserType).HasColumnName("userType").HasDefaultValue(0);
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

            modelBuilder.Entity<DbNote>(entity =>
            {
                entity.ToTable("notes");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id).HasColumnName("Id").HasDefaultValueSql("NEWID()");
                entity.Property(u => u.UserId).HasColumnName("UserId").IsRequired();
                entity.Property(u => u.Title).HasColumnName("Title").HasMaxLength(255).IsRequired();
                entity.Property(u => u.Content).HasColumnName("Content").IsRequired();
                entity.Property(u => u.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("SYSUTCDATETIME()").IsRequired();
                entity.Property(u => u.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("SYSUTCDATETIME()").IsRequired();
                entity.Property(u => u.IsArchived).HasColumnName("IsArchived").HasDefaultValue(false).IsRequired();
                entity.Property(u => u.IsPinned).HasColumnName("IsPinned").HasDefaultValue(false).IsRequired();
                entity.Property(u => u.Style).HasColumnName("Style").HasMaxLength(255).IsRequired(false);

                entity.HasOne<DbUser>()
                      .WithMany()
                      .HasForeignKey(u => u.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
