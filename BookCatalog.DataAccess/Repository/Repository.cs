using BookCatalog.DataAccess.Data;
using BookCatalog.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookCatalog.DataAccess.Repository
{
    

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<T>();
        }
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
           _db.AddRange(entities);
        }

        public T? Get(Expression<Func<T, bool>> filter)
        {
            return _dbSet.Where(filter).FirstOrDefault();
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}
