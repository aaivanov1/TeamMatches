using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamMatches.Domain.Models;

namespace TeamMatches.Infrastructure.Configurations
{
    public class GameConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.ToTable("Games");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.HomeTeamScore).IsRequired();
            builder.Property(x => x.GuestTeamScore).IsRequired();
            builder.Property(x => x.PlayedOnUtc).IsRequired();

            builder.HasOne(x => x.HomeTeam)
                .WithMany()
                .HasForeignKey(x => x.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.GuestTeam)
                .WithMany()
                .HasForeignKey(x => x.GuestTeamId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
