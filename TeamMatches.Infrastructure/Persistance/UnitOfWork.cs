using TeamMatches.Domain.Interfaces;
using TeamMatches.Infrastructure.Repositories;

namespace TeamMatches.Infrastructure.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext _context;
        public UnitOfWork(ApplicationContext context)
        {
            _context = context;
            Teams = new TeamRepository(_context);
            Games = new GameRepository(_context);
        }

        public ITeamRepository Teams { get; private set; }

        public IGameRepository Games { get; private set; }

        public int Complete()
        {
            throw new NotImplementedException();
        }

        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync();
        }

        public async Task DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }
}
