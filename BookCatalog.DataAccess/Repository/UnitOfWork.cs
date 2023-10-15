using BookCatalog.DataAccess.Data;
using BookCatalog.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalog.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            CategoryRepo = new CategoryRepository(db);
            ProductRepo = new ProductRepository(db);
        }
        public ICategoryRepository? CategoryRepo { get; private set; }

		public IProductRepository? ProductRepo { get; private set; }

	    public void Save()
        {
            _db.SaveChanges();
        }
    }
}
