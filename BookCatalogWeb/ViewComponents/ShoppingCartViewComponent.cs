using BookCatalog.DataAccess.Repository.IRepository;
using BookCatalog.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookCatalogWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent 
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync() 
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity!;
            string? userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is not null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart) is null) 
                {
                    HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCartRepo.GetAll(sc => sc.ApplicationUserId == userId).Count());
                }
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else 
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
