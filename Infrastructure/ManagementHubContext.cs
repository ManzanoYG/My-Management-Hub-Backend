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
        }
    }
}
