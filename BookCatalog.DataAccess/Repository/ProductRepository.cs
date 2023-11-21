using BookCatalog.DataAccess.Data;
using BookCatalog.DataAccess.Repository.IRepository;
using BookCatalog.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookCatalog.DataAccess.Repository
{
	public class ProductRepository : Repository<Product>,IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            _db.Update(product);
        }
        public override Product? Get(Expression<Func<Product,bool>> filter, bool isTracked = true) 
        {
            return isTracked ? _db.Products.Include("Category").FirstOrDefault(filter)
                : _db.Products.Include("Category").AsNoTracking().FirstOrDefault(filter);
        }
    }
}
