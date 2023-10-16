using BookCatalog.DataAccess.Data;
using BookCatalog.DataAccess.Repository.IRepository;
using BookCatalog.Models;
using BookCatalog.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace BookCatalogWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> categoryList = _unitOfWork!.CategoryRepo!.GetAll().ToList();
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
			if (GeneralValidator.IsStringTooShort(obj.Name,3))
			{
				ModelState.AddModelError("Author", "Length of name should be at lest 3 symbols!");
			}
			if (ModelState.IsValid)
            {
                TempData["success"] = "Category created successfully!";
                _unitOfWork!.CategoryRepo!.Add(obj);
                _unitOfWork.Save();
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
            Category? category = _unitOfWork!.CategoryRepo!.Get(category => category.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
			if (GeneralValidator.IsStringTooShort(obj.Name, 3))
			{
				ModelState.AddModelError("Author", "Length of name should be at lest 3 symbols!");
			}
			if (ModelState.IsValid)
            {
                TempData["success"] = "Category updated successfully!";
                _unitOfWork!.CategoryRepo!.Update(obj);
                _unitOfWork.Save();
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
            Category? category = _unitOfWork!.CategoryRepo!.Get(category => category.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Delete(Category obj)
        {
            _unitOfWork!.CategoryRepo!.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully!";
            return RedirectToAction("Index", "Category");
        }

    }
}
