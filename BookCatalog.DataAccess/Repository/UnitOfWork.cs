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
            OrderDetailRepo = new OrderDetailRepository(db);
            OrderHeaderRepo = new OrderHeaderRepository(db);
            ApplicationUserRepo = new ApplicationUserRepository(db);
        }
        public ICategoryRepository? CategoryRepo { get; }

        public IProductRepository? ProductRepo { get;}

        public ICompanyRepository? CompanyRepo { get;}
        public IShoppingCartRepository ShoppingCartRepo { get; }

        public IOrderDetailRepository OrderDetailRepo { get; }

        public IOrderHeaderRepository OrderHeaderRepo { get; }
        public IApplicationUserRepository ApplicationUserRepo {  get; }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
