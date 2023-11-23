using BookCatalog.DataAccess.Repository.IRepository;
using BookCatalog.Models;
using BookCatalog.Models.ViewModels;
using BookCatalog.Utility;
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
        public IActionResult Index(string status)
		{
            IEnumerable<OrderHeader> orderHeaders = _unitOfWork!.OrderHeaderRepo!.GetAll(includeProperties: "ApplicationUser");
            IEnumerable<OrderHeader> filteredOrderHeaders = status switch 
            {
                "pending" => orderHeaders.Where(oh => oh.OrderStatus == SD.PaymentStatusDelayedPayment),
                "approved" => orderHeaders.Where(oh => oh.OrderStatus == SD.StatusApproved),
                "inprocess" => orderHeaders.Where(oh => oh.OrderStatus == SD.StatusInProcess),
                "completed" => orderHeaders.Where(oh => oh.OrderStatus == SD.StatusShipped),
                _ => orderHeaders,
            };
            return View(filteredOrderHeaders);
		}
        public IActionResult Details(int id)
        {
            OrderVM orderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeaderRepo.GetAll(oh => oh.Id == id, "ApplicationUser").FirstOrDefault(),
                OrderDetails = _unitOfWork.OrderDetailRepo.GetAll(od => od.OrderHeaderId == id, "Product")
            };
            return View(orderVM);
        }

	}
}
