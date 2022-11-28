using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace WebApplication1.DataAccess.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _DbContext;
        public Repository(DbContext dbContext)
        {
            _DbContext = dbContext;
        }

        public void Add(TEntity entity)
        {
            _DbContext.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _DbContext.AddRange(entities);
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _DbContext.Set<TEntity>().Where(predicate);
        }

        public TEntity Get(int id)
        {
            return _DbContext.Set<TEntity>().Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _DbContext.Set<TEntity>().ToList();
        }

        public void Remove(TEntity entity)
        {
            _DbContext.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _DbContext.Set<TEntity>().RemoveRange(entities);
        }
    }
}
