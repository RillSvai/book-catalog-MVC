using BookCatalog.DataAccess.Repository.IRepository;
using BookCatalog.Models;
using BookCatalog.Models.ViewModels;
using BookCatalog.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.ComponentModel;
using System.Security.Claims;

namespace BookCatalogWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(string status)
		{
            IEnumerable<OrderHeader> orderHeaders = _unitOfWork!.OrderHeaderRepo!.GetAll(includeProperties: "ApplicationUser");
            if (User.IsInRole(SD.Role_Company) || User.IsInRole(SD.Role_Customer))
            {
                ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity!;
                string userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                orderHeaders = orderHeaders.Where(oh => oh.ApplicationUserId == userId);
            }
            IEnumerable<OrderHeader> filteredOrderHeaders = status switch 
            {
                "pending" => orderHeaders.Where(oh => oh.PaymentStatus == SD.PaymentStatusDelayedPayment),
                "approved" => orderHeaders.Where(oh => oh.OrderStatus == SD.StatusApproved),
                "inprocess" => orderHeaders.Where(oh => oh.OrderStatus == SD.StatusInProcess),
                "completed" => orderHeaders.Where(oh => oh.OrderStatus == SD.StatusShipped),
                _ => orderHeaders,
            };
            return View(filteredOrderHeaders);
		}
        public IActionResult Details(int id)
        {
            OrderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeaderRepo.GetAll(oh => oh.Id == id, "ApplicationUser").FirstOrDefault(),
                OrderDetails = _unitOfWork.OrderDetailRepo.GetAll(od => od.OrderHeaderId == id, "Product")
            };
            return View(OrderVM);
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        public IActionResult UpdateOrderDetail() 
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepo.Get(oh => oh.Id == OrderVM.OrderHeader!.Id)!;
            orderHeader.Name = OrderVM.OrderHeader!.Name;
            orderHeader.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeader.City = OrderVM.OrderHeader.City;
            orderHeader.PostalCode = OrderVM.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeader.Carrier = OrderVM.OrderHeader.TrackingNumber;
            }
            _unitOfWork.OrderHeaderRepo.Update(orderHeader);
            _unitOfWork.Save();
            TempData["Success"] = "Order details updated successfully!";

            return RedirectToAction(nameof(Details),new {id = orderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        public IActionResult StartProcessing() 
        {
            _unitOfWork.OrderHeaderRepo.UpdateStatus(OrderVM.OrderHeader!.Id, SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["Success"] = "Order status updated successfully!";
            return RedirectToAction(nameof(Details), new { id = OrderVM.OrderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        public IActionResult ShipOrder() 
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepo.Get(oh => oh.Id == OrderVM.OrderHeader!.Id)!;
            orderHeader.TrackingNumber = OrderVM.OrderHeader!.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader!.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment) 
            {
                orderHeader.PaymentDate = DateTime.Now.AddDays(30);
            }
            _unitOfWork.OrderHeaderRepo.Update(orderHeader);
            _unitOfWork.Save();
            TempData["Success"] = "Order shipped successfully!";
            return RedirectToAction(nameof(Details), new { id = OrderVM.OrderHeader.Id });
        }
        [HttpPost]
        public IActionResult Details()
        {
            OrderVM.OrderHeader = _unitOfWork.OrderHeaderRepo.GetAll(oh => oh.Id == OrderVM.OrderHeader!.Id, "ApplicationUser").FirstOrDefault();
            OrderVM.OrderDetails = _unitOfWork.OrderDetailRepo.GetAll(od => od.OrderHeaderId == OrderVM.OrderHeader!.Id, "Product");
            StripeConfiguration.ApiKey = "sk_test_51OExQdITx7zBE6GxNtwT5vUjjeM84uxLjok8jQkeo4eFysvPRwKkKWFfkVqJbKzwzO8eASuPsex3a5MY15H1Pjl900yUd0CyGz";

            string domain = "https://localhost:7056/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?id={OrderVM.OrderHeader!.Id}",
                CancelUrl = domain + $"admin/order/details?id={OrderVM.OrderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (OrderDetail item in OrderVM.OrderDetails)
            {
                SessionLineItemOptions sessionItem = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "uah",
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = item.Product.Title
                        },
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionItem);
            }
            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeaderRepo.UpdateStripePaymentId(OrderVM.OrderHeader!.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        public IActionResult PaymentConfirmation(int id)
        {
            OrderHeader? orderHeader = _unitOfWork.OrderHeaderRepo.Get(oh => oh.Id == id);
            if (orderHeader!.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                SessionService service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeaderRepo.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeaderRepo.UpdateStatus(id, orderHeader.OrderStatus!, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }
            return View(id);
        }
        public IActionResult CancelOrder() 
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepo.Get(oh => oh.Id == OrderVM.OrderHeader!.Id)!;
            if (orderHeader.PaymentStatus == SD.StatusApproved) 
            {
                RefundCreateOptions options = new RefundCreateOptions()
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId,
                };
                RefundService service = new RefundService();
                Refund refund = service.Create(options);
                _unitOfWork.OrderHeaderRepo.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else 
            {
                _unitOfWork.OrderHeaderRepo.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            _unitOfWork.Save();
            TempData["Success"] = "Order cancelled successfully!";
            return RedirectToAction(nameof(Details), new { id = OrderVM.OrderHeader.Id });
        }
    }
}
