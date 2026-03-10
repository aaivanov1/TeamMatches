using Microsoft.EntityFrameworkCore;
using TeamMatches.Domain.Interfaces;
using TeamMatches.Domain.Models;
using TeamMatches.Infrastructure.Persistance;

namespace TeamMatches.Infrastructure.Repositories
{
    public class TeamRepository : EntityRepository<Team>, ITeamRepository
    {
        public TeamRepository(ApplicationContext context) : base(context)
        {
        }

        public async Task<Team> GetTeamByNameAsync(string name)
        {
            return await _context.Teams.FirstOrDefaultAsync(t => t.Name == name);
        }
    }
}
