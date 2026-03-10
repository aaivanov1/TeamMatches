using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TeamMatches.Domain;
using TeamMatches.Domain.Interfaces;
using TeamMatches.Infrastructure.Persistance;

namespace TeamMatches.Infrastructure.Repositories
{
    public class EntityRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly ApplicationContext _context;
        public EntityRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task InsertAsync(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

            await _context.Set<TEntity>().AddAsync(entity);
        }

        public void Remove(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

            switch (entity)
            {
                case ISoftDeleteEntity softDeleteEntity:
                    softDeleteEntity.IsDeleted = true;
                    _context.Set<TEntity>().Update(entity);
                    break;
                default:
                    _context.Set<TEntity>().Remove(entity);
                    break;
            }
        }

        public void Update(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

            _context.Set<TEntity>().Update(entity);
        }
    }
}
