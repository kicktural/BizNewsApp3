using BizNewsAppDB1.Areas.Admin.PopulityArticle;
using BizNewsAppDB1.Areas.Admin.ProfileViews;
using BizNewsAppDB1.DataServer;
using BizNewsAppDB1.FileHelper;
using BizNewsAppDB1.Models;
using BizNewsAppDB1.ProfileView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BizNewsAppDB1.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    public class ProfileController : Controller
    {
       
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment _env;
		public ProfileController(UserManager<User> userManager, IHttpContextAccessor contextAccessor, IWebHostEnvironment env)
		{
			_userManager = userManager;
			_contextAccessor = contextAccessor;
			_env = env;
		}

		[Authorize]
		public async Task<IActionResult> Index()
        {
            //ProfileView model = new();

            //var userViews = _userManager.Users.Skip(4).Take(1).ToList();       
            //ProfileVM profileVM = new ProfileVM()
            //{
            //    Users = userViews
            //};
            var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            User user = await _userManager.FindByIdAsync(userId);

            ViewData["User"] = user;
            return View(user);
        }



		[HttpGet]
		[Authorize]
		public async Task<IActionResult> EditProfile()
		{
			var Getuser = await _userManager.GetUserAsync(User);
			return View(Getuser);
		}

		[HttpPost]
		public async Task<IActionResult> EditProfile(User user, IFormFile Photo)
		{
			try
			{
				if (user == null)
				{
					return NotFound();
				}
				if (Photo != null)
				{
					user.PhotoUrl = await Photo.SaveFileAsync(_env.WebRootPath);
				}

				var findUser = await _userManager.FindByIdAsync(user.Id);
				findUser.Firstname = user.Firstname;
				findUser.Lastname = user.Lastname;
				findUser.PhotoUrl = user.PhotoUrl;
				findUser.UserName = user.UserName;
				findUser.Email = user.Email;
				var result = await _userManager.UpdateAsync(findUser);
				if (!result.Succeeded)
				{
					var Getuser = await _userManager.GetUserAsync(User);
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError("Error", error.Description);
					}
					return View(Getuser);
				}
				string returnUrl = Request.Query["ReturnUrl"].ToString();
				return Redirect(!string.IsNullOrEmpty(returnUrl) ? returnUrl : "/");
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("Error", ex.Message);
				var Getuser = await _userManager.GetUserAsync(User);
				return View(Getuser);
			}
		}



    }
}
