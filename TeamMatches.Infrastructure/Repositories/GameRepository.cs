using TeamMatches.Domain.Interfaces;
using TeamMatches.Domain.Models;
using TeamMatches.Infrastructure.Persistance;

namespace TeamMatches.Infrastructure.Repositories
{
    public class GameRepository : EntityRepository<Game>, IGameRepository
    {
        public GameRepository(ApplicationContext context) : base(context)
        {
        }
    }
}
