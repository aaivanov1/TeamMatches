namespace TeamMatches.Api.Models
{
    public class UpdateGameModel
    {
        public Guid HomeTeamId { get; set; }

        public Guid GuestTeamId { get; set; }

        public int HomeScore { get; set; }

        public int GuestScore { get; set; }

        public DateTime PlayedOnUtc { get; set; }
    }
}
