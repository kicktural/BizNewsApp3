using BizNewsAppDB1.DataServer;
using BizNewsAppDB1.Models;
using BizNewsAppDB1.ViewsModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BizNewsAppDB1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _context;

        public HomeController(ILogger<HomeController> logger, AppDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(string SEARCH)
        {
            var articles = _context.Articles
               .Where(x => x.IsDelated == false)
               .Include(x => x.Category)
               .Include(x => x.User)
               .Include(x => x.ArticleComments)
               .OrderByDescending(x => x.CreateDate);

            int articleCount = _context.Articles.Count();

            var FeaturedArticles = _context.Articles.Include(x => x.Category)
                .Where(x => x.IsFeatured == true
            && x.Status == false && x.IsDelated == false).OrderByDescending(x => x.CreateDate).Take(4).ToList();

         
            var LatestArticles = _context.Articles.Include(x => x.Category).Include(x => x.User)
            .Where(x => x.IsFeatured == false && x.Status == false
               && x.IsDelated == false).OrderByDescending(x => x.CreateDate).ToList(); //.take(13)

            //if (!string.IsNullOrEmpty(SEARCH))
            //{
            //    SEARCH = SEARCH.ToLower();
            //    LatestArticles = LatestArticles.Where(x => x.Title.ToLower()
            //    .Contains(SEARCH) ||
            //    x.Category.CategoryName.ToLower().Contains(SEARCH) ||
            //    x.Content.ToLower().Contains(SEARCH) ||
            //    x.SeoUrl.ToLower().Contains(SEARCH)).ToList();
            //}

            if (!string.IsNullOrEmpty(SEARCH))
            {
                LatestArticles = LatestArticles.Where(x =>
                    x.Title.Contains(SEARCH, StringComparison.OrdinalIgnoreCase) ||
                    x.Category.CategoryName.Contains(SEARCH, StringComparison.OrdinalIgnoreCase) ||
                    x.SeoUrl.Contains(SEARCH, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            var ActiveLatestArticles1 = _context.Articles.Include(x => x.Category)
            .Where(X => X.Status == true
             && X.IsDelated == false).OrderByDescending(x => x.CreateDate).Take(2).ToList();

            var popArticle = _context.Articles.Where(z => z.IsDelated == false)
                .OrderByDescending(x => x.ViewCount).Take(5).ToList();

            var RecentPost = _context.Articles
             .Where(x => x.Status == false && x.IsFeatured == false && x.IsDelated == false).
             OrderByDescending(x => x.CreateDate).Take(3).ToList();

            HomeVM homeVM = new()
            {
                FeaturedArticles = FeaturedArticles,
                LatestArticles = LatestArticles,
                ActiveLatestArticles = ActiveLatestArticles1,
                PopulaeArticle = popArticle,
                recentPost = RecentPost,
            };

            return View(homeVM);

        }

        [HttpPost]
        public IActionResult Search(string search)
        {
            return Redirect($"/?search={search}");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}