using BookCatalog.DataAccess.Repository.IRepository;
using BookCatalog.Models;
using BookCatalog.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalogWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
		{
            IEnumerable<OrderHeader> orderHeaders = _unitOfWork!.OrderHeaderRepo!.GetAll(includeProperties: "ApplicationUser");
            return View(orderHeaders);
		}

	}
}
