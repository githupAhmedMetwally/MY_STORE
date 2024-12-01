using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyStore.Data.Impelementaion;
using MyStore.Models.Repositories;
using MyStore.Models.ViewModels;
using MyStore.Utilities;
using Stripe;

namespace MyStore.Wb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles ="Admin")]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork unitOfWork;
		[BindProperty]
		public OrderVM OrderVM { get; set; }
		public OrderController(IUnitOfWork unitOfWork)
        {
			this.unitOfWork = unitOfWork;
		}
        public IActionResult Index()
		{
			return View();
		}
		[HttpGet]
		public IActionResult GetData()
		{
			var orderHeaders = unitOfWork.OrderHeader.GetAll(Includeword: "ApplicationUser");
			return Json(new { data = orderHeaders });
		}
		[HttpGet]
		public IActionResult Details(int orderid)
		{
			OrderVM orderVM = new OrderVM()
			{
				OrderHeader = unitOfWork.OrderHeader.GetFirstorDefault(x => x.Id == orderid, Includeword: "ApplicationUser"),
				OrderDetails = unitOfWork.OrderDetails.GetAll(x => x.OrderHeaderId == orderid, Includeword: "Product")
			};
			return View(orderVM);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult UpdateOrderDetails()
		{
			var orderfromdb = unitOfWork.OrderHeader.GetFirstorDefault(u => u.Id == OrderVM.OrderHeader.Id);
			orderfromdb.Name = OrderVM.OrderHeader.Name;
			orderfromdb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
			orderfromdb.Address = OrderVM.OrderHeader.Address;
			orderfromdb.City = OrderVM.OrderHeader.City;

			if (OrderVM.OrderHeader.Carrier != null)
			{
				orderfromdb.Carrier = OrderVM.OrderHeader.Carrier;
			}

			if (OrderVM.OrderHeader.TrakingNumber != null)
			{
				orderfromdb.TrakingNumber = OrderVM.OrderHeader.TrakingNumber;
			}

			unitOfWork.OrderHeader.Update(orderfromdb);
			unitOfWork.Complete();
			TempData["Update"] = "Item has Updated Successfully";
			return RedirectToAction("Details", "Order", new { orderid = orderfromdb.Id });
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult StartProcess()
		{
			unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.Proccessing, null);
			unitOfWork.Complete();

			TempData["Update"] = "Order Status has Updated Successfully";
			return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult StartShip()
		{
			var orderfromdb = unitOfWork.OrderHeader.GetFirstorDefault(x => x.Id == OrderVM.OrderHeader.Id);
			orderfromdb.TrakingNumber = OrderVM.OrderHeader.TrakingNumber;
			orderfromdb.Carrier = OrderVM.OrderHeader.Carrier;
			orderfromdb.OrderStatus = SD.Shipped;
			orderfromdb.ShippingDate = DateTime.Now;

			unitOfWork.OrderHeader.Update(orderfromdb);
			unitOfWork.Complete();
			TempData["Update"] = "Order  has shipped  Successfully";
			return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult CancelOrder()
		{
			var orderfromdb = unitOfWork.OrderHeader.GetFirstorDefault(u => u.Id == OrderVM.OrderHeader.Id);
			if (orderfromdb.PaymentStatus == SD.Approve)
			{
				var option = new RefundCreateOptions
				{
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = orderfromdb.PaymentIntentId
				};

				var service = new RefundService();
				Refund refund = service.Create(option);

				unitOfWork.OrderHeader.UpdateStatus(orderfromdb.Id, SD.Cancelled, SD.Refund);
			}
			else
			{
				unitOfWork.OrderHeader.UpdateStatus(orderfromdb.Id, SD.Cancelled, SD.Cancelled);
			}
			unitOfWork.Complete();

			TempData["Update"] = "Order has Cancelled Successfully";
			return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
		}
	}
}
