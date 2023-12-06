using BizNewsAppDB1.DataServer;
using BizNewsAppDB1.FileHelper;
using BizNewsAppDB1.Helper;
using BizNewsAppDB1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace BizNewsAppDB1.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin, Moderator")]
    public class ArticleController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ArticleController(AppDBContext context, IHttpContextAccessor contextAccessor, UserManager<User> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var articles = _context.Articles.
                Include(z => z.Category)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .Include(x => x.User)
                .OrderByDescending(x => x.CreateDate)
                .Where(z => z.IsDelated == false).ToList();
            return View(articles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var categoryList = _context.Categories.ToList();
            var tagList = _context.Tags.ToList();
            ViewBag.Categories = new SelectList(categoryList, "Id", "CategoryName");
            ViewData["Tags"] = tagList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Article article, IFormFile Photo, List<int> tagIds)
        {
            try
            {


                var categoryList = _context.Categories.ToList();
                var tagList = _context.Tags.ToList();
                ViewBag.Categories = new SelectList(categoryList, "Id", "CategoryName");
                ViewData["Tags"] = tagList;
             

                if (tagIds.Count == 0)
                {
                    ViewBag.TagError = "Select at least one tag ID";
                    return View();
                }

                if (Photo == null)
                {
                    ViewBag.PhotoError = "Article Photo cannot be null!!!";
                    return View();
                }

                article.PhotoUrl = await Photo.SaveFileAsync(_webHostEnvironment.WebRootPath);        
                article.CreateDate = DateTime.Now;
                var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                article.UserId = userId;
                var user = await _userManager.FindByIdAsync(userId);
                article.CreatedBy = user.Firstname;
                article.ViewCount = 0;

                article.SeoUrl = SeoHelper.ReplaceInvalidChars(article.Title);

                //article.Status = false;
                article.IsDelated = false;

                await _context.Articles.AddAsync(article);
                await _context.SaveChangesAsync();

                List<ArticleTag> TagList = new();

                for (int i = 0; i < tagIds.Count; i++)
                {
                    ArticleTag articleTag = new()
                    {
                        ArticleId = article.Id,
                        TagId = tagIds[i]
                    };
                    TagList.Add(articleTag);
                }
                await _context.ArticleTags.AddRangeAsync(TagList);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                return View();
            }
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            var article = _context.Articles
                .Include(x => x.Category)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .FirstOrDefault(x => x.Id == id);
            if (article == null) return NotFound();

            var categories = _context.Categories.ToList();
            var tags = _context.Tags.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
            ViewData["tags"] = tags;

            return View(article);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Article article, IFormFile Photo, List<int> tagIds)
        {
            try
            {
                var categories = _context.Categories.ToList();
                var tags = _context.Tags.ToList();
                ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
                ViewData["tags"] = tags;

                if (tagIds.Count == 0)
                {
                    ViewBag.TagError = "Select at least 1 tag!";
                    return View();
                }

                if (Photo != null)
                {
                    article.PhotoUrl = await Photo.SaveFileAsync(_webHostEnvironment.WebRootPath);
                }

                article.SeoUrl = SeoHelper.ReplaceInvalidChars(article.Title);

                article.UpdateDate = DateTime.Now;
                var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _userManager.FindByIdAsync(userId);
                article.UpdatedBy = user.Firstname;
                var articleTagDelete = _context.ArticleTags.Where(x => x.ArticleId == article.Id).ToList();
                _context.ArticleTags.RemoveRange(articleTagDelete);

                List<ArticleTag> tagList = new();
                for (int i = 0; i < tagIds.Count; i++)
                {
                    ArticleTag articleTag = new()
                    {
                        ArticleId = article.Id,
                        TagId = tagIds[i]
                    };
                    tagList.Add(articleTag);
                }
                await _context.ArticleTags.AddRangeAsync(tagList);
                await _context.SaveChangesAsync();

                _context.Articles.Update(article);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View();
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var article = await _context.Articles.FirstOrDefaultAsync(x => x.Id == id);
            if (article == null) return NotFound();
            return View(article);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id == null) return NotFound();
                var article = _context.Articles.FirstOrDefault(x => x.Id == id);
                if (article == null) return NotFound();        

                var filePath = (_webHostEnvironment.WebRootPath + article.PhotoUrl).ToLower();

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                _context.Articles.Remove(article);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                return View(ex);
            }

        }

        [HttpGet]
        public IActionResult Detail(int? id)
        {
            if (id == null) return NotFound();
            Article artView = _context.Articles.FirstOrDefault(x => x.Id == id);
            if (artView == null) return NotFound();
            return View(artView);
        }
   
    }
}
