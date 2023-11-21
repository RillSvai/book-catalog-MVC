using BookCatalog.DataAccess.Repository.IRepository;
using BookCatalog.Models;
using BookCatalog.Models.ViewModels;
using BookCatalog.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Security.Claims;

namespace BookCatalogWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity!;
            string userId = claimsIdentity!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            IEnumerable<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCartRepo!.GetAll(sc => sc.ApplicationUserId == userId, "Product");
            ShoppingCartVM ShoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCarts = shoppingCarts,
                OrderHeader = new() { OrderTotal = shoppingCarts.Sum(sc => GetPriceBasedOnQuantity(sc) * sc.Count) }     
            };
            return View(ShoppingCartVM);
        }
        public IActionResult Summary() 
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity!;
            string userId = claimsIdentity!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            IEnumerable<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCartRepo!.GetAll(sc => sc.ApplicationUserId == userId, "Product");
            ShoppingCartVM ShoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCarts = shoppingCarts,
                OrderHeader = new() { OrderTotal = shoppingCarts.Sum(sc => GetPriceBasedOnQuantity(sc) * sc.Count) }
            };
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUserRepo.Get(au => au.Id == userId)!;

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber!;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode!;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City!;
            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST() 
        {
			ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity!;
			string userId = claimsIdentity!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
			IEnumerable<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCartRepo!.GetAll(sc => sc.ApplicationUserId == userId, "Product");
            ShoppingCartVM.ShoppingCarts = shoppingCarts;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;
            ShoppingCartVM.OrderHeader.OrderTotal = shoppingCarts.Sum(sc => GetPriceBasedOnQuantity(sc) * sc.Count);
            bool isCustomer = _unitOfWork.ApplicationUserRepo.Get(au => au.Id == userId)!.CompanyId.GetValueOrDefault() == 0;
            if (isCustomer) 
            {
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.StatusPending;
            }
            else 
            {
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
            }
            _unitOfWork.OrderHeaderRepo.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();
            foreach(ShoppingCart cart in ShoppingCartVM.ShoppingCarts) 
            {
                OrderDetail orderDetail = new OrderDetail
                {
                    Price = GetPriceBasedOnQuantity(cart),
                    Count = cart.Count,
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id
                };
                _unitOfWork.OrderDetailRepo.Add(orderDetail);
            }
			_unitOfWork.Save();
			if (isCustomer) 
            {
                //stripe
            }
            return RedirectToAction(nameof(OrderConfirmation), new {id = ShoppingCartVM.OrderHeader.Id});
        }

        public IActionResult OrderConfirmation(int id) 
        {
            return View(id);
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
