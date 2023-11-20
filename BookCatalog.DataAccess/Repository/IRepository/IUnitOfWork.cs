using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalog.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public ICategoryRepository? CategoryRepo { get; }
        public IProductRepository? ProductRepo { get; }
        public ICompanyRepository? CompanyRepo { get; }
        public void Save();
    }
}
