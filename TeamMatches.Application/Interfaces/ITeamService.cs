using TeamMatches.Application.DTOs;

namespace TeamMatches.Application.Interfaces
{
    public interface ITeamService
    {
        Task<IList<TeamDto>> GetAllAsync();

        Task<TeamDto> GetByIdAsync(Guid id);

        Task<TeamDto> CreateAsync(string name);

        Task<TeamDto> UpdateAsync(Guid id, string name);

        Task DeleteAsync(Guid id);
    }
}
