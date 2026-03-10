using TeamMatches.Domain.Models;

namespace TeamMatches.Domain.Interfaces
{
    public interface ITeamRepository : IRepository<Team>
    {
        Task<Team> GetTeamByNameAsync(string name);
    }
}
