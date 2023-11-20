using BookCatalog.Models;

namespace BookCatalog.DataAccess.Repository.IRepository
{
    public interface ICompanyRepository : IRepository<Company>
    {
        public void Update(Company company);
    }
}
