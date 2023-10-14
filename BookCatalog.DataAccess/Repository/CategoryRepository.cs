
using BookCatalog.DataAccess.Data;
using BookCatalog.DataAccess.Repository.IRepository;
using BookCatalog.Models;
using System.Linq.Expressions;

namespace BookCatalog.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db; 
        public CategoryRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }
        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Category category)
        {
            _db.Update(category);
        }
    }
}
