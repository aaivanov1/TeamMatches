using TeamMatches.Application.DTOs;

namespace TeamMatches.Application.Interfaces
{
    public interface IGameService
    {
        Task<IList<GameDto>> GetAllAsync();

        Task<GameDto> GetByIdAsync(Guid id);

        Task<GameDto> CreateAsync(Guid homeTeamId, Guid guestTeamId, int homeCore, int guestScore, DateTime playedAtUtc);

        Task<GameDto> UpdateAsync(Guid id, Guid homeTeamId, Guid guestTeamId, int homeCore, int guestScore, DateTime playedAtUtc);

        Task DeleteAsync(Guid id);
    }
}
