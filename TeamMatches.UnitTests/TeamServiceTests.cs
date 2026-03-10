using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TeamMatches.UnitTests
{
    [TestClass]
    public sealed class TeamServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly TeamService _sut;

        public TeamServiceTests()
        {
            _teamRepositoryMock = new Mock<ITeamRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _sut = new TeamService(
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
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(teams);

            var result = await _sut.GetAllAsync(CancellationToken.None);

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
                .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            var result = await _sut.GetByIdAsync(teamId, CancellationToken.None);

            result.Id.Should().Be(teamId);
            result.Name.Should().Be("Levski");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenTeamDoesNotExist()
        {
            var teamId = Guid.NewGuid();

            _teamRepositoryMock
                .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Team?)null);

            var act = async () => await _sut.GetByIdAsync(teamId, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"*{teamId}*");
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateTeam_WhenNameIsValidAndUnique()
        {
            const string name = "Levski";

            _teamRepositoryMock
                .Setup(x => x.GetByNameAsync(name, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Team?)null);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _sut.CreateAsync(name, CancellationToken.None);

            result.Name.Should().Be(name);

            _teamRepositoryMock.Verify(
                x => x.AddAsync(It.Is<Team>(t => t.Name == name), It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldTrimName_WhenNameHasSurroundingSpaces()
        {
            const string rawName = "   Levski   ";
            const string expectedName = "Levski";

            _teamRepositoryMock
                .Setup(x => x.GetByNameAsync(expectedName, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Team?)null);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _sut.CreateAsync(rawName, CancellationToken.None);

            result.Name.Should().Be(expectedName);

            _teamRepositoryMock.Verify(
                x => x.AddAsync(It.Is<Team>(t => t.Name == expectedName), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public async Task CreateAsync_ShouldThrowValidationException_WhenNameIsEmpty(string invalidName)
        {
            var act = async () => await _sut.CreateAsync(invalidName, CancellationToken.None);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*Team name is required*");

            _teamRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowConflictException_WhenTeamNameAlreadyExists()
        {
            const string name = "Levski";

            _teamRepositoryMock
                .Setup(x => x.GetByNameAsync(name, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Team { Id = Guid.NewGuid(), Name = name });

            var act = async () => await _sut.CreateAsync(name, CancellationToken.None);

            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("*already exists*");

            _teamRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateTeam_WhenInputIsValid()
        {
            var teamId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Old Name" };

            _teamRepositoryMock
                .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            _teamRepositoryMock
                .Setup(x => x.GetByNameAsync("New Name", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Team?)null);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _sut.UpdateAsync(teamId, "New Name", CancellationToken.None);

            result.Name.Should().Be("New Name");
            team.Name.Should().Be("New Name");

            _teamRepositoryMock.Verify(x => x.Update(team), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowNotFoundException_WhenTeamDoesNotExist()
        {
            var teamId = Guid.NewGuid();

            _teamRepositoryMock
                .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Team?)null);

            var act = async () => await _sut.UpdateAsync(teamId, "New Name", CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();

            _teamRepositoryMock.Verify(x => x.Update(It.IsAny<Team>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenNameIsInvalid(string invalidName)
        {
            var teamId = Guid.NewGuid();

            var act = async () => await _sut.UpdateAsync(teamId, invalidName, CancellationToken.None);

            await act.Should().ThrowAsync<ValidationException>();

            _teamRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowConflictException_WhenAnotherTeamHasSameName()
        {
            var teamId = Guid.NewGuid();
            var existingTeam = new Team { Id = teamId, Name = "Old Name" };
            var duplicateTeam = new Team { Id = Guid.NewGuid(), Name = "Levski" };

            _teamRepositoryMock
                .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingTeam);

            _teamRepositoryMock
                .Setup(x => x.GetByNameAsync("Levski", It.IsAny<CancellationToken>()))
                .ReturnsAsync(duplicateTeam);

            var act = async () => await _sut.UpdateAsync(teamId, "Levski", CancellationToken.None);

            await act.Should().ThrowAsync<ConflictException>();

            _teamRepositoryMock.Verify(x => x.Update(It.IsAny<Team>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldAllowSameNameForSameTeam()
        {
            var teamId = Guid.NewGuid();
            var sameTeam = new Team { Id = teamId, Name = "Levski" };

            _teamRepositoryMock
                .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(sameTeam);

            _teamRepositoryMock
                .Setup(x => x.GetByNameAsync("Levski", It.IsAny<CancellationToken>()))
                .ReturnsAsync(sameTeam);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _sut.UpdateAsync(teamId, "Levski", CancellationToken.None);

            result.Name.Should().Be("Levski");
            _teamRepositoryMock.Verify(x => x.Update(sameTeam), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteTeam_WhenTeamExistsAndHasNoMatches()
        {
            var teamId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Levski" };

            _teamRepositoryMock
                .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            _teamRepositoryMock
                .Setup(x => x.HasMatchesAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            await _sut.DeleteAsync(teamId, CancellationToken.None);

            _teamRepositoryMock.Verify(x => x.Remove(team), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowNotFoundException_WhenTeamDoesNotExist()
        {
            var teamId = Guid.NewGuid();

            _teamRepositoryMock
                .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Team?)null);

            var act = async () => await _sut.DeleteAsync(teamId, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();

            _teamRepositoryMock.Verify(x => x.Remove(It.IsAny<Team>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowConflictException_WhenTeamHasMatches()
        {
            var teamId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Levski" };

            _teamRepositoryMock
                .Setup(x => x.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            _teamRepositoryMock
                .Setup(x => x.HasMatchesAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var act = async () => await _sut.DeleteAsync(teamId, CancellationToken.None);

            await act.Should().ThrowAsync<ConflictException>();

            _teamRepositoryMock.Verify(x => x.Remove(It.IsAny<Team>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
