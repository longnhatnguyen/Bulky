using BulkyBook.DataAccess.Entities;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Packaging.Signing;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        //private readonly IProductRepository _ProductRepository;
        //private readonly ApplicationDbContext _applicationDbContext;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            //var ProductList = _unitOfWork.Product.GetAll();
            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return View(productList);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return Json(new { data = productList });
        }

        public IActionResult Create()
        {
			//IEnumerable<SelectListItem> CategoryList = 
   //         //ViewBag.CategoryList = CategoryList;
   //         ViewData["CategoryList"] = CategoryList;
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				}),
			Product = new Product()
            };
			return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductVM obj)
        {
                if (ModelState.IsValid)
                {
                    _unitOfWork.Product.Add(obj.Product);
                    _unitOfWork.Product.Save();
				TempData["success"] = "Category update done";
				return RedirectToAction("Index");
                }
            return View();
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.Product.Get(c => c.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });

        }

        //[HttpDelete]
        //public IActionResult Delete(int? id)
        //{
        //    var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
        //    if (obj == null)
        //    {
        //        return Json(new { success = false, message = "Error while deleting" });
        //    }

        //    var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
        //    if (System.IO.File.Exists(oldImagePath))
        //    {
        //        System.IO.File.Delete(oldImagePath);
        //    }

        //    _unitOfWork.Product.Remove(obj);
        //    _unitOfWork.Save();
        //    return Json(new { success = true, message = "Delete Successful" });

        //}
        public IActionResult UpSert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
			if (id == null || id == 0)
			{
				return View(productVM);
			}
			else
			{
				productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
				return View(productVM);
			}
		}
        [HttpPost]
        public IActionResult UpSert(ProductVM obj, IFormFile? file)
        {
			if (ModelState.IsValid)
			{
				string wwwRootPath = _hostEnvironment.WebRootPath;
				if (file != null)
				{
					string fileName = Guid.NewGuid().ToString();
					var uploads = Path.Combine(wwwRootPath, @"images\products");
					var extension = Path.GetExtension(file.FileName);

					if (obj.Product.ImageUrl != null)
					{
						var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
						if (System.IO.File.Exists(oldImagePath))
						{
							System.IO.File.Delete(oldImagePath);
						}
					}

					using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
					{
						file.CopyTo(fileStreams);
					}
					obj.Product.ImageUrl = @"\images\products\" + fileName + extension;

				}
				if (obj.Product.Id == 0)
				{
					_unitOfWork.Product.Add(obj.Product);
				}
				else
				{
					_unitOfWork.Product.Update(obj.Product);
				}
				_unitOfWork.Save();
				TempData["success"] = "Product created successfully";
				return RedirectToAction("Index");
			}
			return View();
		}
        public async Task<IActionResult> DeleteById(int? id)
        {
            var Product = _unitOfWork.Product.Get(c => c.Id == id);
            if (Product == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Remove(Product);
            _unitOfWork.Product.Save();
            return RedirectToAction("Index", "Product");
        }

        //[HttpPost]
        //public async Task<IActionResult> Edit(Product obj)
        //{
        //    //if (ModelState.IsValid)
        //    //{
        //    //    ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Name.");
        //    //}
        //    //if (ModelState.IsValid)
        //    //{
        //        _unitOfWork.Product.Update(obj);
        //        _unitOfWork.Product.Save();
        //        TempData["success"] = "Product update done";
        //        return RedirectToAction("Index");
        //    //}
        //    //return View(obj);
        //}
    }
}
