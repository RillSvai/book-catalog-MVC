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
            CompanyRepo = new CompanyRepository(db);
            ShoppingCartRepo = new ShoppingCartRepository(db);
        }
        public ICategoryRepository? CategoryRepo { get; }

        public IProductRepository? ProductRepo { get;}

        public ICompanyRepository? CompanyRepo { get;}
        public IShoppingCartRepository ShoppingCartRepo { get; }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
