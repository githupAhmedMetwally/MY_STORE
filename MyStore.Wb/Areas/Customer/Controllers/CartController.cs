using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyStore.Models.Repositories;
using MyStore.Models.ViewModels;
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
	}
}
