using System.Linq.Expressions;

namespace TeamMatches.Domain.Interfaces
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity> GetByIdAsync(Guid id);

        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate);

        Task InsertAsync(TEntity entity);

        void Update(TEntity entity);

        void Remove(TEntity entity);


    }
}
