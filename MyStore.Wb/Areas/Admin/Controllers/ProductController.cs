﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyStore.Data;
using MyStore.Models.Models;
using MyStore.Models.Repositories;
using MyStore.Models.ViewModels;
using static System.Net.Mime.MediaTypeNames;

namespace MyStore.Wb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetData()
        {
            var products = _unitOfWork.Product.GetAll(Includeword: "Category");
            return Json(new { data = products });
        }
        [HttpGet]
        public IActionResult Create()
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductVM productVM,IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var rootPath = _webHostEnvironment.WebRootPath; //wwwroot folder
                if(file != null)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var Upload = Path.Combine(rootPath, @"Images\Product");
                    var ext = Path.GetExtension(file.FileName);
                    using (var filestream = new FileStream(Path.Combine(Upload, fileName + ext), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    productVM.Product.Img = @"Images\Products\" + fileName + ext;
                }
                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Complete();
                TempData["Create"] = "Item has Created Successfully";
                return RedirectToAction("Index");
            }

            return View(productVM.Product);
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                NotFound();
            }
             
            ProductVM productVM = new ProductVM()
            {
                Product = _unitOfWork.Product.GetFirstorDefault(x => x.Id == id),
                CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            return View(productVM);
         
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductVM productVM,IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                var rootPath = _webHostEnvironment.WebRootPath; //wwwroot folder
                if (file != null)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var Upload = Path.Combine(rootPath, @"Images\Product");
                    var ext = Path.GetExtension(file.FileName);
                    //delete oldImg
                    if (productVM.Product.Img != null)
                    {
                        var oldimg = Path.Combine(rootPath, productVM.Product.Img.TrimStart('\\'));
                        if (System.IO.File.Exists(oldimg))
                        {
                            System.IO.File.Delete(oldimg);
                        }
                    }
                    using (var filestream = new FileStream(Path.Combine(Upload, fileName + ext), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    productVM.Product.Img = @"Images\Product\" + fileName + ext;
                }
                _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Complete();
                TempData["Update"] = "Item has Updated Successfully";
                return RedirectToAction("Index");
            }

            return View(productVM.Product);
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productIndb = _unitOfWork.Product.GetFirstorDefault(x => x.Id == id);
            if (productIndb == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }
            _unitOfWork.Product.Remove(productIndb);
            var oldimg = Path.Combine(_webHostEnvironment.WebRootPath, productIndb.Img.TrimStart('\\'));
            if (System.IO.File.Exists(oldimg))
            {
                System.IO.File.Delete(oldimg);
            }
            _unitOfWork.Complete();
            return Json(new { success = true, message = "file has been Deleted" });
        }
    }
}