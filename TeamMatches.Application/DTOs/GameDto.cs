namespace TeamMatches.Application.DTOs
{
    public class GameDto
    {
        public Guid Id { get; set; }

        public Guid HomeTeamId { get; set; }

        public string HomeTeamName { get; set; }

        public Guid GuestTeamId { get; set; }

        public string GuestTeamName { get; set; }

        public int HomeTeamScore { get; set; }

        public int GuestTeamScore { get; set; }

        public DateTime PlaidOnUtc { get; set; }
    }
}
