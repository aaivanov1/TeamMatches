using TeamMatches.Application.DTOs;
using TeamMatches.Application.Interfaces;
using TeamMatches.Domain.Interfaces;

namespace TeamMatches.Application.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LeaderboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<RankDto>> GetRankingsAsync()
        {
            var teams = await _unitOfWork.Teams.GetAllAsync();
            var games = await _unitOfWork.Games.GetAllAsync();

            var rankings = teams.Select(team =>
            {
                var teamMatches = games
                    .Where(m => m.HomeTeamId == team.Id || m.GuestTeamId == team.Id)
                    .ToList();

                var played = 0;
                var wins = 0;
                var draws = 0;
                var losses = 0;
                var goalsScored = 0;
                var goalsAgainst = 0;
                var points = 0;

                foreach (var match in teamMatches)
                {
                    played++;

                    var isHome = match.HomeTeamId == team.Id;
                    var gs = isHome ? match.HomeTeamScore : match.GuestTeamScore;
                    var ga = isHome ? match.GuestTeamScore : match.HomeTeamScore;

                    goalsScored += gs;
                    goalsAgainst += ga;
                    points += CalculatePoints(gs, ga);

                    if (gs > ga) wins++;
                    else if (gs == ga) draws++;
                    else losses++;
                }

                return new RankDto
                {
                    TeamId = team.Id,
                    TeamName = team.Name,
                    GamesPlayed = played,
                    Wins = wins,
                    Draws = draws,
                    Losses = losses,
                    GoalsScored = goalsScored,
                    GoalsAgainst = goalsAgainst,
                    GoalDifference = goalsScored - goalsAgainst,
                    Points = points
                };
            })
            .OrderByDescending(x => x.Points)
            .ThenByDescending(x => x.GoalDifference)
            .ThenByDescending(x => x.GoalsScored)
            .ThenBy(x => x.TeamName)
            .ToList();

            return rankings;
        }

        protected int CalculatePoints(int goalsScored, int goalsAgainst)
        {
            if (goalsScored > goalsAgainst) return 3;
            if (goalsScored == goalsAgainst) return 1;
            return 0;
        }
    }
}
