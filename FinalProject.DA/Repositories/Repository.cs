using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAcess.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _DbContext;
        public Repository(DbContext dbContext)
        {
            _DbContext = dbContext;
        }
        public void AddAsync(TEntity entity)
        {
            _DbContext.AddAsync(entity);
        }
        public void AddRangeAsync(IEnumerable<TEntity> entities)
        {
            _DbContext.AddRangeAsync(entities);
        }
        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _DbContext.Set<TEntity>().Where(predicate).ToListAsync();
        }
        public virtual async Task<TEntity> GetAsync((int, int) id)
        {
            return await _DbContext.Set<TEntity>().FindAsync(id.Item1, id.Item2);
        }
        public virtual async Task<TEntity> GetAsync(int id)
        {
            return await _DbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _DbContext.Set<TEntity>().ToListAsync();
        }
        public void Remove(TEntity entity)
        {
            _DbContext.Set<TEntity>().Remove(entity);
        }
        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _DbContext.Set<TEntity>().RemoveRange(entities);
        }
        public async Task<bool> DoesExistAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _DbContext.Set<TEntity>().AnyAsync(predicate);
        }
    }
}
