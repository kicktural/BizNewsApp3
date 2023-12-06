using BizNewsAppDB1.DataServer;
using BizNewsAppDB1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BizNewsAppDB1.Controllers
{
    public class CommentController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDBContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        public CommentController(UserManager<User> userManager, AppDBContext context, IHttpContextAccessor contextAccessor)
        {
            _userManager = userManager;
            _context = context;
            _contextAccessor = contextAccessor;
        }


    }
}
