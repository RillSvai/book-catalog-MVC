using BookCatalog.DataAccess.Repository.IRepository;
using BookCatalog.Models;
using BookCatalog.Models.ViewModels;
using BookCatalog.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BookCatalogWeb.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Company>? companies = _unitOfWork.CompanyRepo!.GetAll();
            return View(companies);
        }
        public IActionResult Upsert(int? id)
        {
            Company? company = new();
            if (id != 0 && id != null)
            {
                company = _unitOfWork.CompanyRepo!.Get(c => c.Id == id);
            }
            return View(company);
        }
        [HttpGet]
		public IActionResult Delete(int? id)
		{
			Company? company = _unitOfWork.CompanyRepo!.Get(c => c.Id == id);
			_unitOfWork.CompanyRepo.Remove(company!);
			_unitOfWork.Save();
			return RedirectToAction("Index","Company");
		}
		[HttpPost]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id != 0)
                {
                    _unitOfWork.CompanyRepo!.Update(company);
                    TempData["success"] = "Company updated successfully!";
                }
                else
                {
                    _unitOfWork!.CompanyRepo!.Add(company);
                    TempData["success"] = "Company created successfully!";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index", "Company");
            }
            else
            {
                return View(company);
            }
        }
        
    }
}
