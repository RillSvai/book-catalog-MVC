using System.Linq.Expressions;

namespace BookCatalog.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        public IEnumerable<T> GetAll();
        public T? Get(Expression<Func<T,bool>> filter, bool isTracked = false);
        public void Add(T entity);
        public void AddRange(IEnumerable<T> entities);
        public void Remove(T entity);
        public void RemoveRange(IEnumerable<T> entities);
    }
}
