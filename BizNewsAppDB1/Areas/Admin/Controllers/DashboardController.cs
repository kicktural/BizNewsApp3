using BizNewsAppDB1.Areas.Admin.PopulityArticle;
using BizNewsAppDB1.DataServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BizNewsAppDB1.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin, Moderator")]
    public class DashboardController : Controller
    {
        private readonly AppDBContext _context;

        public DashboardController(AppDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {      
            return View();
        }
    }
}
