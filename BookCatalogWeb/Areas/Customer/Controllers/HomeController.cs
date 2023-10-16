using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BookCatalog.Models;
using BookCatalog.DataAccess.Repository.IRepository;

namespace BookCatalogWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View(_unitOfWork.ProductRepo!.GetAll().ToList());
        }
        public IActionResult Details(int? id) 
        {
            return View(_unitOfWork.ProductRepo!.Get(product => product.Id == id));
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