namespace TeamMatches.Application.DTOs
{
    public class RankDto
    {
        public Guid TeamId { get; set; }

        public string TeamName { get; set; }

        public int GamesPlayed { get; set; }

        public int Wins { get; set; }

        public int Losses { get; set; }

        public int Draws { get; set; }

        public int GoalsScored { get; set; }

        public int GoalsAgainst { get; set; }

        public int Points { get; set; }
    }
}
