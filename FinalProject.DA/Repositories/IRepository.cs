using System.Linq.Expressions;

namespace WebApplication1.DataAccess.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetAsync((int, int) id);
        Task<TEntity> GetAsync(int id);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> GetAllAsync();
        void AddAsync(TEntity entity);
        void AddRangeAsync(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        Task<bool> DoesExistAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
