using BookCatalog.DataAccess.Repository.IRepository;
using BookCatalog.Models;
using BookCatalog.Models.ViewModels;
using BookCatalog.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging.Signing;
using System.Text.RegularExpressions;

namespace BookCatalogWeb.Areas.Admin.Controllers
{

	[Area("Admin")]
	public class ProductController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}
		public IActionResult Index()
		{
			List<Product> products = _unitOfWork!.ProductRepo!.GetAll().ToList();
			return View(products);
		}
		public IActionResult Upsert(int? id)
		{
			ProductVM productVM = new ProductVM()
			{
				Product = new Product(),
				CategoryList = _unitOfWork.CategoryRepo!.GetAll().Select(category => new SelectListItem
				{
					Text = category.Name,
					Value = category.Id.ToString(),
				})
			};
			if (id != 0 && id != null) 
			{
				productVM.Product = _unitOfWork.ProductRepo.Get(category => category.Id == id);
			}
			return View(productVM);
		}
		[HttpPost]
		public IActionResult Upsert(ProductVM productVM, IFormFile? file) 
		{
			
			if (GeneralValidator.IsStringTooShort(productVM.Product.Author ?? null,3)) 
			{
				ModelState.AddModelError("Author", "Length of author`s name should be at lest 3 symbols!");
			}
			if (ModelState.IsValid) 
			{
				string wwwRootPath = _webHostEnvironment.WebRootPath;
				if (file != null) 
				{
					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
					string productPath = Path.Combine(wwwRootPath, @"images\product");
					using (var fileStream = new FileStream(Path.Combine(productPath, fileName),FileMode.Create)) 
					{
						file.CopyTo(fileStream);
					}
					productVM.Product.ImageUrl = @"\images\product\" + fileName;
				}
				TempData["success"] = "Product created successfully!";
				_unitOfWork!.ProductRepo!.Add(productVM.Product);
				_unitOfWork.Save();
				return RedirectToAction("Index", "Product");
			}
			else 
			{
				productVM.CategoryList = _unitOfWork.CategoryRepo!.GetAll().Select(category => new SelectListItem
				{
					Text = category.Name,
					Value = category.Id.ToString(),
				});
				return View(productVM);
			}
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
