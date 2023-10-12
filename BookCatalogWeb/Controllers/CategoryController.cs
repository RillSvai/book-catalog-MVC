using BookCatalogWeb.Data;
using BookCatalogWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace BookCatalogWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db) 
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> categoryList = _db.Categories.ToList();
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
				_db.Categories.Add(obj);
				_db.SaveChanges();
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
            Category? category = _db.Categories.FirstOrDefault(category => category.Id == id);
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
				_db.Categories.Update(obj);
				_db.SaveChanges();
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
			Category? category = _db.Categories.FirstOrDefault(category => category.Id == id);
			if (category == null)
			{
				return NotFound();
			}
			return View(category);
		}
		[HttpPost]
		public IActionResult Delete(Category obj)
		{
			_db.Categories.Remove(obj);
			_db.SaveChanges();
			TempData["success"] = "Category deleted successfully!";
			return RedirectToAction("Index", "Category");
		}

	}
}
