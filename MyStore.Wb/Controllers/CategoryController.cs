using Microsoft.AspNetCore.Mvc;
using MyStore.Wb.Models;

namespace MyStore.Wb.Controllers
{
	public class CategoryController : Controller
	{
		private readonly ApplicationDbContext context;

		public CategoryController(ApplicationDbContext context)
        {
			this.context = context;
		}
        public IActionResult Index()
		{
			var result = context.Category.ToList();
			return View(result);
		}
		[HttpGet]
        public IActionResult Create()
        {
			return View();
        }
        [HttpPost]
		[ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
			if (ModelState.IsValid)
			{
                context.Category.Add(category);
                context.SaveChanges();
                TempData["Create"] = "Item has Created Successfully";
                return RedirectToAction("Index");
            }
            
            return View(category);
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if(id==null || id == 0)
            {
                NotFound();
            }
            var categoryId = context.Category.Find(id);
            
            return View(categoryId);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                context.Category.Update(category);
                context.SaveChanges();
                TempData["Update"] = "Item has Updated Successfully";
                return RedirectToAction("Index");
            }
            
            return View(category);
        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                NotFound();
            }
            var categoryId = context.Category.Find(id);
            return View(categoryId);
        }
        [HttpPost]
        public IActionResult DeleteCategory(int? id)
        {
            
            var categoryId = context.Category.Find(id);
            if(categoryId == null){
                NotFound();
            }
           context.Category.Remove(categoryId);
            context.SaveChanges();
            TempData["Delete"] = "Item has Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
