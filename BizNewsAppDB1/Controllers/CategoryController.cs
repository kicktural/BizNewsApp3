using BizNewsAppDB1.DataServer;
using BizNewsAppDB1.ViewsModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BizNewsAppDB1.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDBContext _context;

        public CategoryController(AppDBContext context)
        {
            _context = context;
        }
       
        [HttpGet]
        public IActionResult Category(int id)
        {
            if (id == null) return NotFound();
            var article = _context.Articles
                .Include(x => x.Category)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .Where(x => x.IsDelated == false)
                .FirstOrDefault(x => x.Id == id);
            if (article == null) return NotFound();



            var PopPostCategory = _context.Articles.
                OrderByDescending(x => x.ViewCount).Take(4).ToList();

            var cate = _context.Articles.Include(x => x.Category).OrderByDescending(x => x.CreateDate)
                //.Where(x => x.CategoryId == article.CategoryId || x.CategoryId != article.CategoryId)
                      .Where(x =>x.CategoryId == article.CategoryId && x.Id != article.Id && x.IsFeatured == false && x.IsDelated == false)
                      .Take(3).ToList();

            var FeaturedArticles = _context.Articles.Include(x => x.Category)
                .Where(z => z.IsFeatured == true 
            && z.Status == false && z.IsDelated == false ).OrderByDescending(x => x.CreateDate).Take(4).ToList();

            //ArticleCategory categoryViews = new()
            //{
            //    Articles = article,
            //    PopularPost = PopPostCategory,
            //    RecentPost = cate,
            //};
            ArticleDetailVM articleDetailVM = new()
            {
                Article = article,
                PopularPosts = FeaturedArticles,
                RecentPost = cate,
                PopularCategory = PopPostCategory
            };
            return View(articleDetailVM);
        }
    }
}
