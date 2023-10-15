using BookCatalog.DataAccess.Repository.IRepository;
using BookCatalog.Models;
using BookCatalog.Utility;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Signing;
using System.Text.RegularExpressions;

namespace BookCatalogWeb.Areas.Admin.Controllers
{

	[Area("Admin")]
	public class ProductController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public ProductController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			List<Product> products = _unitOfWork!.ProductRepo!.GetAll().ToList();
			return View(products);
		}
		public IActionResult Create() 
		{
			return View(_unitOfWork?.ProductRepo?.Get(product => product.Id == 1) ?? new Product());
		}
		[HttpPost]
		public IActionResult Create(Product product) 
		{
			if (GeneralValidator.IsStringTooShort(product?.Author,3)) 
			{
				ModelState.AddModelError("Author", "Length of author`s name should be at lest 3 symbols!");
			}
			if (_unitOfWork!.CategoryRepo!.Get(category => category.Id == product!.CategoryId) == default) 
			{
				ModelState.AddModelError("CategoryId", $"Category with id:{product!.CategoryId} not founded");
			}
			if (ModelState.IsValid) 
			{
				TempData["success"] = "Product created successfully!";
				_unitOfWork!.ProductRepo!.Add(product!);
				_unitOfWork.Save();
				return RedirectToAction("Index", "Product");
			}
			return View();
		}
		public IActionResult Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			Product? product = _unitOfWork!.ProductRepo!.Get(category => category.Id == id);
			if (product == null)
			{
				return NotFound();
			}
			return View(product);
		}
		[HttpPost]
		public IActionResult Edit(Product product)
		{
			if (GeneralValidator.IsStringTooShort(product?.Author, 3))
			{
				ModelState.AddModelError("Author", "Length of author`s name should be at lest 3 symbols!");
			}
			if (_unitOfWork!.CategoryRepo!.Get(category => category.Id == product!.CategoryId) == default)
			{
				ModelState.AddModelError("CategoryId", $"Category with id:{product!.CategoryId} not founded");
			}
			if (ModelState.IsValid)
			{
				TempData["success"] = "Product updated successfully!";
				_unitOfWork!.ProductRepo!.Update(product);
				_unitOfWork.Save();
				return RedirectToAction("Index", "Product");
			}
			return View();
		}
		public IActionResult Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			Product? product = _unitOfWork!.ProductRepo!.Get(category => category.Id == id);
			if (product == null)
			{
				return NotFound();
			}
			return View(product);
		}
		[HttpPost]
		public IActionResult Delete(Product obj)
		{
			_unitOfWork!.ProductRepo!.Remove(obj);
			_unitOfWork.Save();
			TempData["success"] = "Product deleted successfully!";
			return RedirectToAction("Index", "Product");
		}
	}
}
