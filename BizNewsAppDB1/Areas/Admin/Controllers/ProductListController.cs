using BizNewsAppDB1.Areas.Admin.PopulityArticle;
using BizNewsAppDB1.DataServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BizNewsAppDB1.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles ="Admin, Moderator")]
    public class ProductListController : Controller
    {
        private readonly AppDBContext _context;

        public ProductListController(AppDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var articlePop = _context.Articles.OrderByDescending(x => x.ViewCount).Where(z => z.IsDelated == false).Take(5).ToList();
            var Featured = _context.Articles.OrderByDescending(z => z.IsFeatured).OrderByDescending(a => a.CreateDate).Where(z => z.IsFeatured == true).Take(3).ToList();
            var Recent = _context.Articles.OrderByDescending(z => z.CreateDate).Where(z => z.Status == false).Take(3).ToList();
            PopArticleVM popArticleVM = new()
            {
                Article = articlePop,
                PopArticle = articlePop,
                IsFeatured = Featured,
                RecentProduct = Recent,
            };
            return View(popArticleVM);
        }
    }
}
