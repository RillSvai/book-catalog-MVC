using BookCatalog.DataAccess.Data;
using BookCatalog.DataAccess.Repository.IRepository;
using BookCatalog.Models;
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
        public virtual void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
           _db.AddRange(entities);
        }

        public virtual T? Get(Expression<Func<T, bool>> filter, bool isTracked = false)
        {
            IQueryable<T> query = isTracked ? _dbSet : _dbSet.AsNoTracking();
            return query.Where(filter).FirstOrDefault();
        }

        public virtual IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string includeProperties = "")
        {
            IQueryable<T> query = _dbSet;
            if (filter is not null) 
            {
                query = query.Where(filter);
            }
            foreach (string property in includeProperties.Split(",", StringSplitOptions.RemoveEmptyEntries)) 
            {
                query = query.Include(property);
            }
            return query;
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
