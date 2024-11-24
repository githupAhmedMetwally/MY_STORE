using Microsoft.AspNetCore.Mvc;
using MyStore.Data;
using MyStore.Models.Models;
using MyStore.Models.Repositories;

namespace MyStore.Wb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var result = _unitOfWork.Category.GetAll();
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
                _unitOfWork.Category.Add(category);
                _unitOfWork.Complete();
                TempData["Create"] = "Item has Created Successfully";
                return RedirectToAction("Index");
            }

            return View(category);
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                NotFound();
            }
            var categoryId = _unitOfWork.Category.GetFirstorDefault(x => x.Id == id);

            return View(categoryId);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(category);
                _unitOfWork.Complete();
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
            var categoryId = _unitOfWork.Category.GetFirstorDefault(x => x.Id == id);
            return View(categoryId);
        }
        [HttpPost]
        public IActionResult DeleteCategory(int? id)
        {

            var categoryId = _unitOfWork.Category.GetFirstorDefault(x => x.Id == id);
            if (categoryId == null)
            {
                NotFound();
            }
            _unitOfWork.Category.Remove(categoryId);
            _unitOfWork.Complete();
            TempData["Delete"] = "Item has Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
