using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BookCatalog.Models;
using BookCatalog.DataAccess.Repository.IRepository;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using BookCatalog.Utility;
using BookCatalog.Models.ViewModels;

namespace BookCatalogWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController( IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity!;
            string? userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is not null) 
            {
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCartRepo.GetAll(sc => sc.ApplicationUserId == userId).Count());
            }
            return View(_unitOfWork.ProductRepo!.GetAll().ToList());
        }
        public IActionResult Details(int id)
        {
            ShoppingCart shoppingCart = new ShoppingCart()
            {
                Product = _unitOfWork.ProductRepo!.Get(product => product.Id == id)!,
                ProductId = id,
                Count = 1,
            };
            return View(shoppingCart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details (ShoppingCart shoppingCart) 
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity!;
            shoppingCart.Id = 0;
            string userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            shoppingCart.ApplicationUserId = userId;
            _unitOfWork.ShoppingCartRepo!.Add(shoppingCart);
            _unitOfWork.Save();
            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCartRepo.GetAll(sc => sc.ApplicationUserId == userId).Count());
            TempData["success"] = "Cart successfully updated!";

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}