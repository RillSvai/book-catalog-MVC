using BookCatalog.DataAccess.Repository.IRepository;
using BookCatalog.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalogWeb.Areas.Admin.Controllers
{
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
		{
			return View();
		}

		#region API Calls
		[HttpGet]
		public IActionResult GetAll()
		{
			List<OrderHeader> products = _unitOfWork!.OrderHeaderRepo!.GetAll(includeProperties: "ApplicationUser").ToList();
			return Json(new { data = products });
		}
		#endregion
	}
}
