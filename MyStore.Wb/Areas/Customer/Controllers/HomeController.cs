using Microsoft.AspNetCore.Mvc;
using MyStore.Models.Repositories;
using MyStore.Models.ViewModels;

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
        public IActionResult Index()
        {
            var result = unitOfWork.Product.GetAll();
            return View(result);
        }
        public IActionResult Details(int id)
        {
            ShoppingCartVM obj = new ShoppingCartVM()
            {
                Product = unitOfWork.Product.GetFirstorDefault(x => x.Id == id, Includeword: "Category"),
                Count = 1
            };
            
            return View(obj);
        }
    }
}
