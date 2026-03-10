using FluentAssertions;
using Moq;
using TeamMatches.Application.Services;
using TeamMatches.Domain.Interfaces;
using TeamMatches.Domain.Models;

namespace TeamMatches.Tests
{
    public class LeaderboardServiceTests
    {
        private readonly Mock<IGameRepository> _gameRepositoryMock;
        private readonly Mock<ITeamRepository> _teamRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly LeaderboardService _lut;
        public LeaderboardServiceTests()
        {
            _gameRepositoryMock = new Mock<IGameRepository>();
            _teamRepositoryMock = new Mock<ITeamRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _lut = new LeaderboardService(
                _teamRepositoryMock.Object,
                _gameRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }
        [Fact]
        public async Task GetRankingsAsync_ShouldReturnZeroStats_ForTeamsWithoutMatches()
        {
            var teamA = new Team { Id = Guid.NewGuid(), Name = "Levski" };
            var teamB = new Team { Id = Guid.NewGuid(), Name = "CSKA" };

            _teamRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<Team> { teamA, teamB });

            _gameRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<Game>());

            var result = await _lut.GetRankingsAsync();

            result.Should().HaveCount(2);
            result.Should().OnlyContain(x =>
                x.GamesPlayed == 0 &&
                x.Wins == 0 &&
                x.Draws == 0 &&
                x.Losses == 0 &&
                x.Points == 0);
        }

        [Fact]
        public async Task GetRankingsAsync_ShouldCalculateWinDrawLossAndPointsCorrectly()
        {
            var teamA = new Team { Id = Guid.NewGuid(), Name = "Levski" };
            var teamB = new Team { Id = Guid.NewGuid(), Name = "CSKA" };
            var teamC = new Team { Id = Guid.NewGuid(), Name = "Ludogorets" };

            var games = new List<Game>
        {
            new()
            {
                Id = Guid.NewGuid(),
                HomeTeamId = teamA.Id,
                GuestTeamId = teamB.Id,
                HomeTeamScore = 2,
                GuestTeamScore = 1
            },
            new()
            {
                Id = Guid.NewGuid(),
                HomeTeamId = teamA.Id,
                GuestTeamId = teamC.Id,
                HomeTeamScore = 1,
                GuestTeamScore = 1
            },
            new()
            {
                Id = Guid.NewGuid(),
                HomeTeamId = teamB.Id,
                GuestTeamId = teamC.Id,
                HomeTeamScore = 0,
                GuestTeamScore = 3
            }
        };

            _teamRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<Team> { teamA, teamB, teamC });

            _gameRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(games);

            var result = await _lut.GetRankingsAsync();

            var levski = result.Single(x => x.TeamId == teamA.Id);
            var cska = result.Single(x => x.TeamId == teamB.Id);
            var ludogorets = result.Single(x => x.TeamId == teamC.Id);

            levski.GamesPlayed.Should().Be(2);
            levski.Wins.Should().Be(1);
            levski.Draws.Should().Be(1);
            levski.Losses.Should().Be(0);
            levski.GoalsScored.Should().Be(3);
            levski.GoalsAgainst.Should().Be(2);
            levski.GoalDifference.Should().Be(1);
            levski.Points.Should().Be(4);

            cska.GamesPlayed.Should().Be(2);
            cska.Wins.Should().Be(0);
            cska.Draws.Should().Be(0);
            cska.Losses.Should().Be(2);
            cska.Points.Should().Be(0);

            ludogorets.GamesPlayed.Should().Be(2);
            ludogorets.Wins.Should().Be(1);
            ludogorets.Draws.Should().Be(1);
            ludogorets.Losses.Should().Be(0);
            ludogorets.Points.Should().Be(4);
        }
    }
}
