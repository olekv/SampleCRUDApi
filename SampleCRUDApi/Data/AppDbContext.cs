using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SampleCRUDApi.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<CompanyEntity> Companies { get; set; }
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CompanyEntity>().HasIndex(x => x.Name).IsUnique();
        }

        public override int SaveChanges()
        {
            SetTimestamps();
            
            return base.SaveChanges();
        }
        
        public override Task<int> SaveChangesAsync(CancellationToken token = default)
        {
            SetTimestamps();
            
            return base.SaveChangesAsync(token);
        }

        private void SetTimestamps()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(x => x.Entity is BaseEntity &&
                            (x.State == EntityState.Added || x.State == EntityState.Modified));
            
            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;
                if (entry.State == EntityState.Added) entity.CreatedAtUtc = DateTime.UtcNow;
                else entity.UpdatedAtUtc = DateTime.UtcNow;
            }
        }
    }
}