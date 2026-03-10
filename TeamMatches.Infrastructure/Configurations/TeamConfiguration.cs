using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamMatches.Domain.Models;

namespace TeamMatches.Infrastructure.Configurations
{
    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.ToTable("Teams");
            builder.HasKey(t => t.Id);
            builder.Property(x => x.Name).IsRequired();
            builder.HasIndex(x => x.Name)
             .IsUnique()
             .HasFilter("[IsDeleted] = 0");
            builder.Property(x => x.CreatedOnUtc).IsRequired();
            builder.Property(x => x.UpdatedOnUtc).IsRequired();
        }
    }
}
