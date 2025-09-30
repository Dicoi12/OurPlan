using Microsoft.EntityFrameworkCore;
using OurPlan.Entity;
using System;

namespace OurPlan.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
