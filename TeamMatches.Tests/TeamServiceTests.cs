using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TeamMatches.Application.DTOs;
using TeamMatches.Application.Mappings;
using TeamMatches.Application.Services;
using TeamMatches.Domain.Exceptions;
using TeamMatches.Domain.Interfaces;
using TeamMatches.Domain.Models;

namespace TeamMatches.Tests
{
    public class TeamServiceTests
    {
        private readonly Mock<ITeamRepository> _teamRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly TeamService _sut;
        public TeamServiceTests()
        {
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            }, new LoggerFactory());

            var mapper = mockMapper.CreateMapper();

            _teamRepositoryMock = new Mock<ITeamRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _sut = new TeamService(
                mapper,
                _teamRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnMappedTeams()
        {
            var teams = new List<Team>
        {
            new() { Id = Guid.NewGuid(), Name = "Levski" },
            new() { Id = Guid.NewGuid(), Name = "CSKA" }
        };

            _teamRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(teams);

            var result = await _sut.GetAllAsync();

            result.Should().HaveCount(2);
            result.Should().ContainEquivalentOf(new TeamDto { Id = teams[0].Id, Name = "Levski" });
            result.Should().ContainEquivalentOf(new TeamDto { Id = teams[1].Id, Name = "CSKA" });
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnTeam_WhenTeamExists()
        {
            var teamId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Levski" };

            _teamRepositoryMock
                .Setup(x => x.GetByIdAsync(teamId))
                .ReturnsAsync(team);

            var result = await _sut.GetByIdAsync(teamId);

            result.Id.Should().Be(teamId);
            result.Name.Should().Be("Levski");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenTeamDoesNotExist()
        {
            var teamId = Guid.NewGuid();

            _teamRepositoryMock
                .Setup(x => x.GetByIdAsync(teamId))
                .ReturnsAsync((Team?)null);

            var act = async () => await _sut.GetByIdAsync(teamId);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"*{teamId}*");
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateTeam_WhenNameIsValidAndUnique()
        {
            const string name = "Levski";

            _teamRepositoryMock
                .Setup(x => x.GetTeamByNameAsync(name))
                .ReturnsAsync((Team?)null);

            _unitOfWorkMock
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _sut.CreateAsync(name);

            result.Name.Should().Be(name);

            _teamRepositoryMock.Verify(
                x => x.InsertAsync(It.Is<Team>(t => t.Name == name)),
                Times.Once);

            _unitOfWorkMock.Verify(
                x => x.CompleteAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }


        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public async Task CreateAsync_ShouldThrowValidationException_WhenNameIsEmpty(string invalidName)
        {
            var act = async () => await _sut.CreateAsync(invalidName);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("Team name cannot be empty");

            _teamRepositoryMock.Verify(
                x => x.InsertAsync(It.IsAny<Team>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.CompleteAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowConflictException_WhenTeamNameAlreadyExists()
        {
            const string name = "Levski";

            _teamRepositoryMock
                .Setup(x => x.GetTeamByNameAsync(name))
                .ReturnsAsync(new Team { Id = Guid.NewGuid(), Name = name });

            var act = async () => await _sut.CreateAsync(name);

            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("Team with this name already exists");

            _teamRepositoryMock.Verify(
                x => x.InsertAsync(It.IsAny<Team>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.CompleteAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateTeam_WhenInputIsValid()
        {
            var teamId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Old Name" };

            _teamRepositoryMock
                .Setup(x => x.GetByIdAsync(teamId))
                .ReturnsAsync(team);

            _teamRepositoryMock
                .Setup(x => x.GetTeamByNameAsync("New Name"))
                .ReturnsAsync((Team?)null);

            _unitOfWorkMock
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _sut.UpdateAsync(teamId, "New Name");

            result.Name.Should().Be("New Name");
            team.Name.Should().Be("New Name");

            _teamRepositoryMock.Verify(x => x.Update(team), Times.Once);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowNotFoundException_WhenTeamDoesNotExist()
        {
            var teamId = Guid.NewGuid();

            _teamRepositoryMock
                .Setup(x => x.GetByIdAsync(teamId))
                .ReturnsAsync((Team?)null);

            var act = async () => await _sut.UpdateAsync(teamId, "New Name");

            await act.Should().ThrowAsync<NotFoundException>();

            _teamRepositoryMock.Verify(x => x.Update(It.IsAny<Team>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenNameIsInvalid(string invalidName)
        {
            var teamId = Guid.NewGuid();

            var act = async () => await _sut.UpdateAsync(teamId, invalidName);

            await act.Should().ThrowAsync<ValidationException>();

            _teamRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowConflictException_WhenAnotherTeamHasSameName()
        {
            var teamId = Guid.NewGuid();
            var existingTeam = new Team { Id = teamId, Name = "Old Name" };
            var duplicateTeam = new Team { Id = Guid.NewGuid(), Name = "Levski" };

            _teamRepositoryMock
                .Setup(x => x.GetByIdAsync(teamId))
                .ReturnsAsync(existingTeam);

            _teamRepositoryMock
                .Setup(x => x.GetTeamByNameAsync("Levski"))
                .ReturnsAsync(duplicateTeam);

            var act = async () => await _sut.UpdateAsync(teamId, "Levski");

            await act.Should().ThrowAsync<ConflictException>();

            _teamRepositoryMock.Verify(x => x.Update(It.IsAny<Team>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteTeam_WhenTeamExists()
        {
            var teamId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Levski" };

            _teamRepositoryMock
                .Setup(x => x.GetByIdAsync(teamId))
                .ReturnsAsync(team);

            _unitOfWorkMock
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            await _sut.DeleteAsync(teamId);

            _teamRepositoryMock.Verify(x => x.Remove(team), Times.Once);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowNotFoundException_WhenTeamDoesNotExist()
        {
            var teamId = Guid.NewGuid();

            _teamRepositoryMock
                .Setup(x => x.GetByIdAsync(teamId))
                .ReturnsAsync((Team?)null);

            var act = async () => await _sut.DeleteAsync(teamId);

            await act.Should().ThrowAsync<NotFoundException>();

            _teamRepositoryMock.Verify(x => x.Remove(It.IsAny<Team>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}