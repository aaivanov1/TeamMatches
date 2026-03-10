using TeamMatches.Domain.Interfaces;

namespace TeamMatches.Domain.Models
{
    public class Game : BaseEntity, ISoftDeleteEntity
    {
        public Guid HomeTeamId { get; set; }

        public Guid GuestTeamId { get; set; }

        public int HomeTeamScore { get; set; }

        public int GuestTeamScore { get; set; }

        public Team? HomeTeam { get; set; }

        public Team? GuestTeam { get; set; }

        public DateTime PlayedOnUtc { get; set; }

        public bool IsDeleted { get; set; }
    }
}
