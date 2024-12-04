using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyStore.Models.Models;
using MyStore.Models.Repositories;
using MyStore.Utilities;
using System.Security.Claims;
using X.PagedList;

namespace MyStore.Wb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index(int ? page)
        {
            var pageNumber = page ?? 1;
            int pageSize = 8;
            var result = unitOfWork.Product.GetAll().ToPagedList(pageNumber,pageSize);
            return View(result);
        }
        [HttpGet]
        public IActionResult Details(int ProductId)
        {
            ShoppingCart obj = new ShoppingCart()
            {
                ProductId=ProductId,
                Product = unitOfWork.Product.GetFirstorDefault(x => x.Id == ProductId, Includeword: "Category"),
                Count = 1
            };
            
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
           shoppingCart.ApplicationUserId = claim.Value;

            ShoppingCart cartobj = unitOfWork.ShoppingCart.GetFirstorDefault(
                x => x.ApplicationUserId == claim.Value && x.ProductId == shoppingCart.ProductId
                );
            if (cartobj == null)
            {
                unitOfWork.ShoppingCart.Add(shoppingCart);
                unitOfWork.Complete();
                HttpContext.Session.SetInt32(SD.SessionKey,
                    unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).ToList().Count());
     
            }
            else
            {
                unitOfWork.ShoppingCart.IncreaseCount(cartobj, shoppingCart.Count);
                unitOfWork.Complete();
            }
            
            

            return RedirectToAction("Index");
        }
    }
}
