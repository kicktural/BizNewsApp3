using BizNewsAppDB1.DataServer;
using BizNewsAppDB1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BizNewsAppDB1.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin, Moderator")]
    public class TagController : Controller
    {
        private readonly AppDBContext _context;

        public TagController(AppDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var TagViews = _context.Tags.ToList();
            return View(TagViews);
        }



        //public async Task<IActionResult> TagSearch(string Empsearch)
        //{
        //    ViewData["Gettagnames"] = Empsearch;
        //    var emoquery = from x in _context.Tags select x;
        //    if (!String.IsNullOrEmpty(Empsearch))
        //    {
        //        emoquery = emoquery.Where(x => x.TagName.Contains(Empsearch));
        //    }
        //    return View(await emoquery.AsNoTracking().ToListAsync());
        //}


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Tag tag)
        {

            try
            {
                var NoName = _context.Tags.FirstOrDefault(x =>x.TagName == tag.TagName);
                if (NoName != null)
                {
                    ViewBag.Errors = "It is impossible to create two things with the same name!";
                    return View(NoName);
                }
                var Tags = _context.Tags.FirstOrDefault(x => x.Id == tag.Id);
                if (Tags != null) return View();
                _context.Tags.Add(tag);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult Update(int? id)
        {
            if (id == null) return NotFound();
            var tags = _context.Tags.FirstOrDefault(x => x.Id == id);
            if (tags == null) return NotFound();
            return View(tags);
        }

        [HttpPost]
        public IActionResult Update(Tag tag)
        {     
            var SecondTag = _context.Tags.FirstOrDefault(x => x.TagName == tag.TagName);
            if (SecondTag != null)
            {
                ViewBag.Tagexist = "There is a tag with this name";
                return View(SecondTag);
            }
            _context.Tags.Update(tag);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public IActionResult Detail(int? id)
        {
            if (id == null) return NotFound();
            var detail = _context.Tags.FirstOrDefault(x => x.Id == id);
            if (detail == null) return NotFound();
            return View(detail);
        }

        [HttpGet]
        public IActionResult Remove(int? id)
        {
            if (id == null) return NotFound();
            var remove = _context.Tags.FirstOrDefault(c => c.Id == id);
            if (remove == null) return NotFound();
            return View(remove);
        }



        [HttpPost]
        public async Task<IActionResult> Remove(Tag tag)
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
