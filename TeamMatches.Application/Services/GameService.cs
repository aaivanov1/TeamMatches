using AutoMapper;
using TeamMatches.Application.DTOs;
using TeamMatches.Application.Interfaces;
using TeamMatches.Domain.Exceptions;
using TeamMatches.Domain.Interfaces;
using TeamMatches.Domain.Models;

namespace TeamMatches.Application.Services
{
    public class GameService : IGameService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public GameService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<GameDto> CreateAsync(Guid homeTeamId, Guid guestTeamId, int homeScore, int guestScore, DateTime playedOnUtc)
        {
            await ValidateMatchAsync(homeTeamId, guestTeamId, homeScore, guestScore);

            var game = new Game
            {
                Id = Guid.NewGuid(),
                HomeTeamId = homeTeamId,
                GuestTeamId = guestTeamId,
                HomeTeamScore = homeScore,
                GuestTeamScore = guestScore,
                PlayedOnUtc = playedOnUtc
            };

            await _unitOfWork.Games.InsertAsync(game);
            await _unitOfWork.CompleteAsync();

            var created = await _unitOfWork.Games.GetByIdAsync(game.Id);
            return _mapper.Map<GameDto>(created);
        }


        public async Task DeleteAsync(Guid id)
        {
            var game = await _unitOfWork.Games.GetByIdAsync(id);
            if (game is null)
                throw new NotFoundException($"Game with id '{id}' was not found.");

            _unitOfWork.Games.Remove(game);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IList<GameDto>> GetAllAsync()
        {
            var games = await _unitOfWork.Games.GetAllAsync();
            return games.Select(_mapper.Map<GameDto>).ToList();
        }

        public async Task<GameDto> GetByIdAsync(Guid id)
        {
            var game = await _unitOfWork.Games.GetByIdAsync(id);
            if (game is null)
                throw new NotFoundException($"Game with id '{id}' was not found.");

            return _mapper.Map<GameDto>(game);
        }

        public async Task<GameDto> UpdateAsync(Guid id, Guid homeTeamId, Guid guestTeamId, int homeScore, int guestScore, DateTime playedOnUtc)
        {
            await ValidateMatchAsync(homeTeamId, guestTeamId, homeScore, guestScore);

            var game = await _unitOfWork.Games.GetByIdAsync(id);
            if (game is null)
                throw new NotFoundException($"Game with id '{id}' was not found");

            game.HomeTeamId = homeTeamId;
            game.GuestTeamId = guestTeamId;
            game.HomeTeamScore = homeScore;
            game.GuestTeamScore = guestScore;
            game.PlayedOnUtc = playedOnUtc;

            _unitOfWork.Games.Update(game);
            await _unitOfWork.CompleteAsync();

            var updated = await _unitOfWork.Games.GetByIdAsync(id);
            return _mapper.Map<GameDto>(updated);
        }

        private async Task ValidateMatchAsync(
            Guid homeTeamId,
            Guid guestTeamId,
            int homeScore,
            int guestScore)
        {
            if (homeTeamId == guestTeamId)
                throw new ValidationException("Home team and away team must be different.");

            if (homeScore < 0 || guestScore < 0)
                throw new ValidationException("Scores cannot be negative.");

            var homeTeam = await _unitOfWork.Teams.GetByIdAsync(homeTeamId);
            if (homeTeam is null)
                throw new ValidationException("Home team does not exist.");

            var awayTeam = await _unitOfWork.Teams.GetByIdAsync(guestTeamId);
            if (awayTeam is null)
                throw new ValidationException("Away team does not exist.");
        }
    }
}
