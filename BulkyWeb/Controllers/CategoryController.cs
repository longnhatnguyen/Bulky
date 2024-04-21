using BulkyBook.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public CategoryController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public IActionResult Index()
        {
            var categoryList = _applicationDbContext.Categories.ToList();
            return View(categoryList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if(ModelState.IsValid)
            {
                var isExist = _applicationDbContext.Categories.Any(n => n.Name == category.Name);
                if (!isExist)
                {
                    _applicationDbContext.Categories.Add(category);
                    await _applicationDbContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("name", "The Name has Exist.");
                    return View();
                }
            }
            return View();
        }
        public async Task<IActionResult> Delete(int? id)
        {
            var category = await _applicationDbContext.Categories.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (category == null)
            {
                return NotFound();
            }
            _applicationDbContext.Categories.Remove(category);
            await _applicationDbContext.SaveChangesAsync();
            return RedirectToAction("Index", "Category");
        }

		public async Task<IActionResult> Edit(int? id)
		{
			var category = await _applicationDbContext.Categories.Where(c => c.Id == id).FirstOrDefaultAsync();
			if (category == null)
			{
				return NotFound();
			}
            return View(category);
		}
		public async Task<IActionResult> DeleteById(int? id)
		{
			var category = await _applicationDbContext.Categories.Where(c => c.Id == id).FirstOrDefaultAsync();
			if (category == null)
			{
				return NotFound();
			}
			_applicationDbContext.Categories.Remove(category);
			await _applicationDbContext.SaveChangesAsync();
			return RedirectToAction("Index", "Category");
		}

		[HttpPost]
		public async Task<IActionResult> Edit(Category obj)
		{
			if (obj.Name == obj.DisplayOrder.ToString())
			{
				ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Name.");
			}
			if (ModelState.IsValid)
			{
                _applicationDbContext.Categories.Update(obj);
				await _applicationDbContext.SaveChangesAsync();
                TempData["success"] = "Category update done";
				return RedirectToAction("Index");
			}
			return View(obj);
		}
	}
}
