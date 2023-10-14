using BookCatalog.DataAccess.Data;
using BookCatalog.DataAccess.Repository.IRepository;
using BookCatalog.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalogWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepo;
        public CategoryController(ICategoryRepository categoryRepo) 
        {
            _categoryRepo = categoryRepo;
        }
        public IActionResult Index()
        {
            List<Category> categoryList = _categoryRepo.GetAll().ToList();
            return View(categoryList);
        }
        [HttpGet]
        public IActionResult Create() 
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj) 
        {
            if (ModelState.IsValid) 
            {
                TempData["success"] = "Category created successfully!";
				_categoryRepo.Add(obj);
				_categoryRepo.Save();
				return RedirectToAction("Index", "Category");
			}
            return View();
        }
		public IActionResult Edit(int? id)
		{
            if (id == null) 
            {
                return NotFound();
            }
			Category? category = _categoryRepo.Get(category => category.Id == id);
            if (category == null) 
            {
                return NotFound();
            }
			return View(category);
		}
		[HttpPost]
		public IActionResult Edit(Category obj)
		{
			if (ModelState.IsValid)
			{
				TempData["success"] = "Category updated successfully!";
                _categoryRepo.Update(obj);
                _categoryRepo.Save();
                return RedirectToAction("Index", "Category");
			}
			return View();
		}
		public IActionResult Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			Category? category = _categoryRepo.Get(category => category.Id == id);
            if (category == null)
			{
				return NotFound();
			}
			return View(category);
		}
		[HttpPost]
		public IActionResult Delete(Category obj)
		{
			_categoryRepo.Remove(obj);
			_categoryRepo.Save();
			TempData["success"] = "Category deleted successfully!";
			return RedirectToAction("Index", "Category");
		}

	}
}
