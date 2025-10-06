using Microsoft.EntityFrameworkCore;
using OurPlan.Entity;
using OurPlan.Entity.Audit;
using OurPlan.Services.Interfaces;
using System;

namespace OurPlan.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService)
           : base(options)
        {

            _currentUserService = currentUserService;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }

        public override int SaveChanges()
        {
            ApplyAuditInformation();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInformation();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditInformation()
        {
            var entries = ChangeTracker.Entries<BaseAuditEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = _currentUserService.UserId;
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedBy = _currentUserService.UserId;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}
