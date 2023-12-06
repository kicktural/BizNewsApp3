using BizNewsAppDB1.DataServer;
using BizNewsAppDB1.Models;
using BizNewsAppDB1.ViewsModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BizNewsAppDB1.Controllers
{
    public class ArticleController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public ArticleController(AppDBContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public IActionResult Detail(int id, string search)
        {
            if (id == null) return NotFound();
            var article = _context.Articles
                .Include(x => x.Category)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .Include(x => x.ArticleComments)
                .Where(x => x.IsDelated == false)
                .FirstOrDefault(x => x.Id == id);
            if (article == null) return NotFound();

            var Cookie = _contextAccessor.HttpContext.Request.Cookies["Views"];
            string[] findCookie = { "" };

            if (Cookie != null)
            {
                findCookie = Cookie.Split("-").ToArray();
            }

            if (!findCookie.Contains(article.Id.ToString()))
            {
                Response.Cookies.Append($"Views", $"{Cookie}-{article.Id}",
                    new CookieOptions
                    {
                        Secure = true,
                        Expires = DateTime.Now.AddYears(6),
                        HttpOnly = true
                    });

                article.ViewCount += 1;
                _context.Articles.Update(article);
                _context.SaveChanges();
            }

            //var similarArticle = _context.Articles.Include(x => x.Category).Where(x => x.CategoryId == article.Id).Take(1).SingleOrDefault();
            var popArt = _context.Articles.OrderByDescending(x => x.ViewCount).Take(3).ToList();

            var articleComments = _context.ArticleComments.Include(x => x.Article).Include(x => x.User)
              .Where(x => x.ArticleId == article.Id).ToList();

            var similarArticles = _context.Articles.Include(x => x.Category)
           .Where(x => x.CategoryId == article.CategoryId /*|| x.CategoryId != article.CategoryId*/ && x.Id != article.Id && x.Status == false && x.IsFeatured == true && x.IsDelated == false).
            OrderByDescending(z => z.CreateDate).Take(1)
           .ToList();


            var similarArticlesTake2 = _context.Articles.Include(x => x.Category)
           .Where(z => z.CategoryId == article.CategoryId && z.Id != article.Id && z.Status == false && z.IsDelated == false && z.IsFeatured == false).OrderByDescending(z => z.CreateDate).Take(1)
           .ToList();


            var ArtCommentPublishDate = _context.ArticleComments.Include(x => x.Article).OrderByDescending(x => x.PublishDate).Where(x => x.Article.IsDelated == false).Take(3).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                ArtCommentPublishDate = ArtCommentPublishDate.Where(x => x.Article.Title.ToLower().Contains(search)).ToList();
            }


            var articlestatus = _context.Articles.Include(x => x.Category).OrderByDescending(x => x.CreateDate)
                .Where(x => x.Status == true && x.IsFeatured == false).Take(2).ToList();

            var recentPost = _context.Articles
                .Where(x => x.Status == false && x.IsFeatured == false && x.IsDelated == false).
                OrderByDescending(x => x.CreateDate).Take(3).ToList();

            ArticleDetailVM articleDetailVM = new()
            {
                Article = article,
                SimilarArticles = similarArticles,
                PopularPosts = popArt,
                ArticleComments = articleComments,
                RecentPost = recentPost,
                SimilarArticleTake2 = similarArticlesTake2,
                ArtCommentPublishdate = ArtCommentPublishDate,
                ArticlesStatus = articlestatus
            };

            return View(articleDetailVM);
        }


        //[HttpPost]
        //public IActionResult Search(string search)
        //{
        //    return Redirect($"/?search={search}");
        //}


        [HttpPost]
        public async Task<IActionResult> AddComment(ArticleComment articleComment, int articleId, string seoUrl)
        {


            var article = _context.Articles
             .Where(x => x.IsDelated == false)
             .Include(x => x.ArticleComments)
             .FirstOrDefault(x => x.Id == articleId);
            if (article == null)
            {
                return NotFound();
            }
            try
            {
                articleComment.ArticleId = articleId;
                articleComment.PublishDate = DateTime.Now;
                articleComment.UserId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                await _context.ArticleComments.AddAsync(articleComment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Detail", new { Id = articleId, SeoUrl = seoUrl });
            }
            catch (Exception)
            {
                return RedirectToAction("Detail", new { Id = articleId, SeoUrl = seoUrl });                
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int commentId, int articleId, string seoUrl)
        {
            var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ViewBag.User = userId;
            var articleComment = _context.ArticleComments.FirstOrDefault(x => x.Id == commentId);
            if (articleComment.UserId == userId)
            {
                _context.ArticleComments.Remove(articleComment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Detail", new { Id = articleId, SeoUrl = seoUrl });
        }
    }
}
