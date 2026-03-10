using Microsoft.EntityFrameworkCore;
using TeamMatches.Domain.Models;

namespace TeamMatches.Infrastructure.Persistance
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<Team> Teams => Set<Team>();
        public DbSet<Game> Games => Set<Game>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
            modelBuilder.Entity<Game>().HasQueryFilter(r => !r.IsDeleted);
            modelBuilder.Entity<Team>().HasQueryFilter(r => !r.IsDeleted);
            base.OnModelCreating(modelBuilder);
        }
    }
}
