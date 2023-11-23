using BookCatalog.DataAccess.Repository.IRepository;
using BookCatalog.Models;
using BookCatalog.Models.ViewModels;
using BookCatalog.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging.Signing;
using System.Data;
using System.Text.RegularExpressions;

namespace BookCatalogWeb.Areas.Admin.Controllers
{

	[Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
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
			IEnumerable<Product> products = _unitOfWork.ProductRepo!.GetAll(includeProperties: "Category");
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
				productVM.Product = _unitOfWork.ProductRepo!.Get(category => category.Id == id)!;
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

					if (!string.IsNullOrEmpty(productVM.Product.ImageUrl)) 
					{
						var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
						if (System.IO.File.Exists(oldImagePath)) 
						{
							System.IO.File.Delete(oldImagePath);
						}

					}
					using (var fileStream = new FileStream(Path.Combine(productPath, fileName),FileMode.Create)) 
					{
						file.CopyTo(fileStream);
					}
					productVM.Product.ImageUrl = @"\images\product\" + fileName;
				}
				if (productVM.Product.Id != 0) 
				{
					_unitOfWork.ProductRepo!.Update(productVM.Product);
					TempData["success"] = "Product updated successfully!";
				}
				else 
				{
					_unitOfWork!.ProductRepo!.Add(productVM.Product);
					TempData["success"] = "Product created successfully!";
				}
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
			Product? product = _unitOfWork.ProductRepo!.Get(product => product.Id == id);
			string wwwRootPath = _webHostEnvironment.WebRootPath;
			var oldImagePath = Path.Combine(wwwRootPath, product!.ImageUrl.TrimStart('\\'));
			if (System.IO.File.Exists(oldImagePath))
			{
				System.IO.File.Delete(oldImagePath);
			}
			_unitOfWork.ProductRepo.Remove(product);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}
	}
}
