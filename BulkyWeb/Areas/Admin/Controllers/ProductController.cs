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
            var ProductList = _unitOfWork.Product.GetAll();
           
            return View(ProductList);
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
        public async Task<IActionResult> Delete(int? id)
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
        public IActionResult UpSert(ProductVM productVM, IFormFile? file)
        {
			if (ModelState.IsValid)
			{
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    productVM.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }
                _unitOfWork.Product.Add(productVM.Product);
				_unitOfWork.Product.Save();
				TempData["success"] = "Category update done";
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
