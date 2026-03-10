using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TeamMatches.Application.Mappings;
using TeamMatches.Application.Services;
using TeamMatches.Domain.Exceptions;
using TeamMatches.Domain.Interfaces;
using TeamMatches.Domain.Models;

namespace TeamMatches.Tests
{
    public class GameServiceTests
    {
        private readonly Mock<IGameRepository> _gameRepositoryMock;
        private readonly Mock<ITeamRepository> _teamRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly GameService _gut;
        public GameServiceTests()
        {
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            }, new LoggerFactory());

            var mapper = mockMapper.CreateMapper();

            _gameRepositoryMock = new Mock<IGameRepository>();
            _teamRepositoryMock = new Mock<ITeamRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _gut = new GameService(
                _teamRepositoryMock.Object,
                _gameRepositoryMock.Object,
                mapper,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnMappedMatches()
        {
            var homeTeamId = Guid.NewGuid();
            var guestTeamId = Guid.NewGuid();

            var games = new List<Game>
        {
            new()
            {
                Id = Guid.NewGuid(),
                HomeTeamId = homeTeamId,
                GuestTeamId = guestTeamId,
                HomeTeamScore = 2,
                GuestTeamScore = 1,
                PlayedOnUtc = DateTime.UtcNow,
                HomeTeam = new Team { Id = homeTeamId, Name = "Levski" },
                GuestTeam = new Team { Id = guestTeamId, Name = "CSKA" }
            }
        };

            _gameRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(games);

            var result = await _gut.GetAllAsync();

            result.Should().HaveCount(1);
            result[0].HomeTeamName.Should().Be("Levski");
            result[0].GuestTeamName.Should().Be("CSKA");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMatch_WhenMatchExists()
        {
            var gameId = Guid.NewGuid();
            var game = new Game
            {
                Id = gameId,
                HomeTeamId = Guid.NewGuid(),
                GuestTeamId = Guid.NewGuid(),
                HomeTeamScore = 1,
                GuestTeamScore = 0,
                PlayedOnUtc = DateTime.UtcNow
            };

            _gameRepositoryMock
                .Setup(x => x.GetByIdAsync(gameId))
                .ReturnsAsync(game);

            var result = await _gut.GetByIdAsync(gameId);

            result.Id.Should().Be(gameId);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenGameDoesNotExist()
        {
            var gameId = Guid.NewGuid();

            _gameRepositoryMock
                .Setup(x => x.GetByIdAsync(gameId))
                .ReturnsAsync((Game?)null);

            var act = async () => await _gut.GetByIdAsync(gameId);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateGame_WhenInputIsValid()
        {
            var homeTeamId = Guid.NewGuid();
            var guestTeamId = Guid.NewGuid();

            _teamRepositoryMock
                .Setup(x => x.GetByIdAsync(homeTeamId))
                .ReturnsAsync(new Team { Id = homeTeamId, Name = "Levski" });

            _teamRepositoryMock
                .Setup(x => x.GetByIdAsync(guestTeamId))
                .ReturnsAsync(new Team { Id = guestTeamId, Name = "CSKA" });

            _unitOfWorkMock
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _gameRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => new Game
                {
                    Id = id,
                    HomeTeamId = homeTeamId,
                    GuestTeamId = guestTeamId,
                    HomeTeamScore = 2,
                    GuestTeamScore = 1,
                    PlayedOnUtc = new DateTime(2026, 1, 1),
                    HomeTeam = new Team { Id = homeTeamId, Name = "Levski" },
                    GuestTeam = new Team { Id = guestTeamId, Name = "CSKA" }
                });

            var result = await _gut.CreateAsync(
                homeTeamId,
                guestTeamId,
                2,
                1,
                new DateTime(2026, 1, 1));

            result.HomeTeamScore.Should().Be(2);
            result.GuestTeamScore.Should().Be(1);

            _gameRepositoryMock.Verify(
                x => x.InsertAsync(It.Is<Game>(m =>
                    m.HomeTeamId == homeTeamId &&
                    m.GuestTeamId == guestTeamId &&
                    m.HomeTeamScore == 2 &&
                    m.GuestTeamScore == 1)));

            _unitOfWorkMock.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task CreateAsync_ShouldThrowValidationException_WhenTeamDoesNotExist()
        {
            var homeTeamId = Guid.NewGuid();
            var awayTeamId = Guid.NewGuid();

            _teamRepositoryMock
                .Setup(x => x.GetByIdAsync(homeTeamId))
                .ReturnsAsync((Team?)null);

            var act = async () => await _gut.CreateAsync(
                homeTeamId,
                awayTeamId,
                1,
                0,
                DateTime.UtcNow);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Team does not exist.");

            _unitOfWorkMock.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateGame_WhenInputIsValid()
        {
            var gameId = Guid.NewGuid();
            var homeTeamId = Guid.NewGuid();
            var guestTeamId = Guid.NewGuid();

            var existingMatch = new Game
            {
                Id = gameId,
                HomeTeamId = Guid.NewGuid(),
                GuestTeamId = Guid.NewGuid(),
                HomeTeamScore = 0,
                GuestTeamScore = 0,
                PlayedOnUtc = new DateTime(2025, 1, 1)
            };

            _teamRepositoryMock
                .Setup(x => x.GetByIdAsync(homeTeamId))
                .ReturnsAsync(new Team { Id = homeTeamId, Name = "Levski" });

            _teamRepositoryMock
                .Setup(x => x.GetByIdAsync(guestTeamId))
                .ReturnsAsync(new Team { Id = guestTeamId, Name = "CSKA" });

            _gameRepositoryMock
                .Setup(x => x.GetByIdAsync(gameId))
                .ReturnsAsync(existingMatch);

            _unitOfWorkMock
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var updatedDate = new DateTime(2026, 2, 2);

            var result = await _gut.UpdateAsync(
                gameId,
                homeTeamId,
                guestTeamId,
                3,
                2,
                updatedDate);

            result.HomeTeamScore.Should().Be(3);
            result.GuestTeamScore.Should().Be(2);

            existingMatch.HomeTeamId.Should().Be(homeTeamId);
            existingMatch.GuestTeamId.Should().Be(guestTeamId);
            existingMatch.HomeTeamScore.Should().Be(3);
            existingMatch.GuestTeamScore.Should().Be(2);
            existingMatch.PlayedOnUtc.Should().Be(updatedDate);

            _gameRepositoryMock.Verify(x => x.Update(existingMatch), Times.Once);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteGamme_WhenGameExists()
        {
            var gameId = Guid.NewGuid();
            var game = new Game { Id = gameId };

            _gameRepositoryMock
                .Setup(x => x.GetByIdAsync(gameId))
                .ReturnsAsync(game);

            _unitOfWorkMock
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            await _gut.DeleteAsync(gameId);

            _gameRepositoryMock.Verify(x => x.Remove(game), Times.Once);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowNotFoundException_WhenGameDoesNotExist()
        {
            var gameId = Guid.NewGuid();

            _gameRepositoryMock
                .Setup(x => x.GetByIdAsync(gameId))
                .ReturnsAsync((Game?)null);

            var act = async () => await _gut.DeleteAsync(gameId);

            await act.Should().ThrowAsync<NotFoundException>();

            _gameRepositoryMock.Verify(x => x.Remove(It.IsAny<Game>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

    }
}
