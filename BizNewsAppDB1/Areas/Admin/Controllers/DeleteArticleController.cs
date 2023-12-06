using BizNewsAppDB1.DataServer;
using BizNewsAppDB1.FileHelper;
using BizNewsAppDB1.Helper;
using BizNewsAppDB1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BizNewsAppDB1.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin")]
    public class DeleteArticleController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<User> _userManager;
        public DeleteArticleController(AppDBContext context, IHttpContextAccessor contextAccessor, IWebHostEnvironment webHostEnvironment, UserManager<User> userManager)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var articles = _context.Articles.Include(x => x.Category)
                .Include(x => x.ArticleTags).ThenInclude(x => x.Tag).Include(z => z.User)
                .Where(x => x.IsDelated == true).OrderByDescending(z => z.CreateDate).ToList();
            return View(articles);
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
                    ViewBag.TagError = "Choose at least 1 tag!";
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

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            var article = _context.Articles.FirstOrDefault(x => x.Id == id);
            if (article == null) return NotFound();
            return View(article);
        }

        [HttpPost]
        public IActionResult Delete(Article article)
        {
            try
            {

                if (article == null) return NotFound();
                var DelArticle = _context.Articles.Remove(article);
                if (DelArticle == null) return NotFound();

                var filePath = (_webHostEnvironment.WebRootPath + article.PhotoUrl).ToLower();

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                return View();
            }
        }
    }
}
