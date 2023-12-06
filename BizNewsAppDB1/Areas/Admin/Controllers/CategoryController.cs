using BizNewsAppDB1.DataServer;
using BizNewsAppDB1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Protocol;

namespace BizNewsAppDB1.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    public class CategoryController : Controller
    {
        private readonly AppDBContext _context;

        public CategoryController(AppDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var category = _context.Categories.ToList();
            return View(category);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();       
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(Category category)
        {
            try
            {
             
                var first = _context.Categories.FirstOrDefault(x =>x.CategoryName == category.CategoryName);
                if (first != null)
                {
                    ViewBag.Error = "It is impossible to create two things with the same name!";
                    return View();
                }       
                    category.CreateDate = DateTime.Now;
                    var Addcategory = _context.Categories.Add(category);
                    _context.SaveChanges();
                    return RedirectToAction("Index");

            }
            catch (Exception ex)
            {

                ViewBag.Errors = ex.Message;
                return View();
            }
           
        }


        [HttpGet]
        public IActionResult Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var serach = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (serach == null) return NotFound();
            return View(serach);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Category category)
        {
           
            var SecondCate = _context.Categories.FirstOrDefault(x =>x.CategoryName == category.CategoryName);
            if (SecondCate != null)
            {
                ViewBag.CategoryExist = "There is a Category with this name!!";
                return View(SecondCate);
            }
            category.UpdateDate = DateTime.Now;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");          
                  
        }

        [HttpGet]
        public IActionResult Detail(int? id)
        {
            if (id == null) return NotFound();
            var deta = _context.Categories.FirstOrDefault(x => x.Id == id);
            if (deta == null) return NotFound();
            return View(deta);
        }


        [HttpGet]
        public IActionResult Remove(int? id)
        {
            if (id == null) return NotFound();
            var remove = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (remove == null) return NotFound();
            return View(remove);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Remove(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");           
        }

    }
}
