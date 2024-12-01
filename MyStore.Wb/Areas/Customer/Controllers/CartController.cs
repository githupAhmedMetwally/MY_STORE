using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyStore.Data.Impelementaion;
using MyStore.Models.Models;
using MyStore.Models.Repositories;
using MyStore.Models.ViewModels;
using MyStore.Utilities;
using Stripe.Checkout;
using System.Security.Claims;

namespace MyStore.Wb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public ShoppingCartVM ShoppingCartVM  { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                CartList = unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value,Includeword:"Product")
            };
            foreach (var item in ShoppingCartVM.CartList)
            {
                ShoppingCartVM.TotalCarts += (item.Count * item.Product.Price);
            }
            return View(ShoppingCartVM);
        }
        public IActionResult Plus(int cartId)
        {
            var shoppingcart=unitOfWork.ShoppingCart.GetFirstorDefault(x=>x.Id==cartId);
            unitOfWork.ShoppingCart.IncreaseCount(shoppingcart, 1);
            unitOfWork.Complete();
            return RedirectToAction("Index");
        }
		public IActionResult Minus(int cartId)
		{
			var shoppingcart = unitOfWork.ShoppingCart.GetFirstorDefault(x => x.Id == cartId);
            if (shoppingcart.Count <= 1)
            {
                unitOfWork.ShoppingCart.Remove(shoppingcart);
                unitOfWork.Complete();
                return RedirectToAction("Index", "Home");
            }
            else
            {
				unitOfWork.ShoppingCart.DecreaseCount(shoppingcart, 1);

			}
			unitOfWork.Complete();
			return RedirectToAction("Index");
		}
		public IActionResult Remove(int cartId)
		{
			var shoppingcart = unitOfWork.ShoppingCart.GetFirstorDefault(x => x.Id == cartId);
			unitOfWork.ShoppingCart.Remove(shoppingcart);
			unitOfWork.Complete();
			return RedirectToAction("Index");
		}


        //summary
        [HttpGet]
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                CartList = unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, Includeword: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = unitOfWork.ApplicationUser.GetFirstorDefault(x => x.Id == claim.Value);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.Address = ShoppingCartVM.OrderHeader.ApplicationUser.Address;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;

            foreach (var item in ShoppingCartVM.CartList)
            {
                ShoppingCartVM.OrderHeader.TotalPrice += (item.Count * item.Product.Price);
            }

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult POSTSummary(ShoppingCartVM ShoppingCartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM.CartList = unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, Includeword: "Product");


            ShoppingCartVM.OrderHeader.OrderStatus = SD.Pending; //لسا متعملش
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.Pending;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;


            foreach (var item in ShoppingCartVM.CartList)
            {
                ShoppingCartVM.OrderHeader.TotalPrice += (item.Count * item.Product.Price);
            }

            unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            unitOfWork.Complete();

            foreach (var item in ShoppingCartVM.CartList)
            {
                OrderDetails orderDetail = new OrderDetails()
                {
                    ProductId = item.ProductId,
					OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = item.Product.Price,
                    Count = item.Count
                };

                unitOfWork.OrderDetails.Add(orderDetail);
                unitOfWork.Complete();
            }

            var domain = "http://localhost:5049/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),

                Mode = "payment",
                SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain + $"customer/cart/index",
            };

            foreach (var item in ShoppingCartVM.CartList)
            {
                var sessionlineoption = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                        },
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionlineoption);
            }


            var service = new SessionService();
            Session session = service.Create(options);
            ShoppingCartVM.OrderHeader.SessionId = session.Id;

            unitOfWork.Complete();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

            //unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.CartsList);
            //unitOfWork.Complete();
            //return RedirectToAction("Index", "Home");

        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = unitOfWork.OrderHeader.GetFirstorDefault(u => u.Id == id);
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                unitOfWork.OrderHeader.UpdateStatus(id, SD.Approve, SD.Approve);
                orderHeader.PaymentIntentId = session.PaymentIntentId;
                unitOfWork.Complete();
            }
            List<ShoppingCart> shoppingcarts = unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            
            unitOfWork.ShoppingCart.RemoveRange(shoppingcarts);
            unitOfWork.Complete();
            return View(id);
        }

    }
}
