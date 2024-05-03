using BulkyBook.DataAccess.Entities;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        //private readonly IProductRepository _ProductRepository;
        //private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var ProductList = _unitOfWork.Product.GetAll();
            return View(ProductList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product Product)
        {
            if (ModelState.IsValid)
            {
                //var isExist = _unitOfWork.Product.Get(n => n.Name == Product.Name);
                if (ModelState.IsValid)
                {
                    _unitOfWork.Product.Add(Product);
                    _unitOfWork.Product.Save();
                    return RedirectToAction("Index");
                }
                //else
                //{
                //    ModelState.AddModelError("name", "The Name has Exist.");
                //    return View();
                //}
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

        public async Task<IActionResult> Edit(int? id)
        {
            var Product = _unitOfWork.Product.Get(c => c.Id == id);
            if (Product == null)
            {
                return NotFound();
            }
            return View(Product);
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

        [HttpPost]
        public async Task<IActionResult> Edit(Product obj)
        {
            //if (ModelState.IsValid)
            //{
            //    ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Name.");
            //}
            //if (ModelState.IsValid)
            //{
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Product.Save();
                TempData["success"] = "Product update done";
                return RedirectToAction("Index");
            //}
            //return View(obj);
        }
    }
}
