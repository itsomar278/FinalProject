using System.Linq.Expressions;

namespace WebApplication1.DataAccess.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity Get((int, int) id);
        TEntity Get(int id);
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> GetAll();
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        bool DoesExist(Expression<Func<TEntity, bool>> predicate);
    }
}
