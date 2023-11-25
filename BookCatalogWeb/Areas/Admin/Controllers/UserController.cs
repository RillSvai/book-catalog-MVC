using BookCatalog.DataAccess.Repository.IRepository;
using BookCatalog.Models;
using BookCatalog.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalogWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            IEnumerable<ApplicationUser> users = _unitOfWork.ApplicationUserRepo.GetAll(includeProperties: "Company");
            return View(users);
        }
        public IActionResult LockUnlock(string id)
        {
            if (_unitOfWork.ApplicationUserRepo.GetRole(id) == SD.Role_Admin)
            {
                TempData["error"] = "You cannot change this for yourself or other admins!";
                return RedirectToAction(nameof(Index));
            }
            _unitOfWork.ApplicationUserRepo.LockUnlock(id);
            TempData["success"] = "Operation was successfull!";
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
	}
}
