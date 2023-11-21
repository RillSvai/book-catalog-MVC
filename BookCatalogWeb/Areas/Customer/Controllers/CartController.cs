using BookCatalog.DataAccess.Repository.IRepository;
using BookCatalog.Models;
using BookCatalog.Models.ViewModels;
using BookCatalog.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookCatalogWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity!;
            string userId = claimsIdentity!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            IEnumerable<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCartRepo!.GetAll(sc => sc.ApplicationUserId == userId, "Product");
            ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCarts = shoppingCarts,
                OrderHeader = new() { OrderTotal = shoppingCarts.Sum(sc => GetPriceBasedOnQuantity(sc) * sc.Count) }     
            };
            return View(shoppingCartVM);
        }
        public IActionResult Summary() 
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity!;
            string userId = claimsIdentity!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            IEnumerable<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCartRepo!.GetAll(sc => sc.ApplicationUserId == userId, "Product");
            ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCarts = shoppingCarts,
                OrderHeader = new() { OrderTotal = shoppingCarts.Sum(sc => GetPriceBasedOnQuantity(sc) * sc.Count) }
            };
            shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUserRepo.Get(au => au.Id == userId)!;

            shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
            shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber!;
            shoppingCartVM.OrderHeader.PostalCode = shoppingCartVM.OrderHeader.ApplicationUser.PostalCode!;
            shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.ApplicationUser.City!;
            return View(shoppingCartVM);
        }
        public IActionResult Plus (int cartId) 
        {
            ShoppingCart changedCart = _unitOfWork.ShoppingCartRepo!.Get(sc => sc.Id == cartId)!;
            changedCart.Count++;
            _unitOfWork.ShoppingCartRepo.Update(changedCart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus (int cartId) 
        {
            ShoppingCart changedCart = _unitOfWork.ShoppingCartRepo!.Get(sc => sc.Id == cartId)!;
            if (changedCart.Count == 1) 
            {
                _unitOfWork.ShoppingCartRepo.Remove(changedCart);
            }
            else 
            {
                changedCart.Count--;
                _unitOfWork.ShoppingCartRepo.Update(changedCart);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartId) 
        {
            ShoppingCart changedCart = _unitOfWork.ShoppingCartRepo!.Get(sc => sc.Id == cartId)!;
            _unitOfWork.ShoppingCartRepo.Remove(changedCart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        private decimal GetPriceBasedOnQuantity(ShoppingCart shoppingCart) => shoppingCart.Count switch
        {
            >= 1 and <= 49 => shoppingCart.Product.Price,
            >= 50 and <= 99 => shoppingCart.Product.Price50,
            _ => shoppingCart.Product.Price100
        };
    }
}
