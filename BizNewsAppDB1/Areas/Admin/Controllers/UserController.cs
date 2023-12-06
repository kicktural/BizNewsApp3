using BizNewsAppDB1.Helper;
using BizNewsAppDB1.Models;
using BizNewsAppDB1.ViewModelVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis;
using System.Security.Claims;

namespace BizNewsAppDB1.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _contextAccessor;
		private readonly IWebHostEnvironment _env;
		public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor contextAccessor, IWebHostEnvironment env)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_contextAccessor = contextAccessor;
			_env = env;
		}

		public IActionResult Index()
        {
            var userViews = _userManager.Users.ToList();
            return View(userViews);
        }

        [HttpGet]
        public async Task<IActionResult> AddRole(string id)
        {
            if (id == null) return NotFound();
            User user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var role = (await _userManager.GetRolesAsync(user)).ToList();
            var roless = _roleManager.Roles.Select(X => X.Name).ToList();

            RoleModelVM roleModelVM = new()
            {
                User = user,
                Roles = roless.Except(role),
            };
            return View(roleModelVM);
        }
        [Authorize (Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddRole(string id, string role)
        {
            if (id == null) return NotFound();
            var user = (await _userManager.FindByIdAsync(id));
            if (user == null) return NotFound();

            var addrole  = await _userManager.AddToRoleAsync(user, role);

            if (!addrole.Succeeded)
            {
                ModelState.AddModelError("Error", "Role");
            }
            return RedirectToAction("Index");
        }
       
        public async Task<IActionResult> Edit(string Userid)
        {
            if (Userid == null) return NotFound();
            User user = await _userManager.FindByIdAsync(Userid);
            if (user == null) return NotFound();
            return View(user);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string userid, string role)
        {
            if (userid == null || role == null) return NotFound();
            var user = await _userManager.FindByIdAsync(userid);
            if (user == null) return NotFound();

            IdentityResult result = await _userManager.RemoveFromRoleAsync(user, role);

            if (!result.Succeeded)
            {
                ViewBag.Error = "Error Role!";
                return View();
            }
            return RedirectToAction("Index");
        }




		// TOdo =======================================================

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> UserInfo()
		{
			var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
			User user = await _userManager.FindByIdAsync(userId);

			return View(user);
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> UserInfo(string userId, string name, string surname, string email, string phoneNumber, string aboutAuthor, IFormFile Photo)
		{
			var user = await _userManager.FindByIdAsync(userId);

			if (user == null)
			{
				// User not found
				return NotFound();
			}

			user.Firstname = name;
			user.UserName = surname;
			user.Email = email;
			user.PhoneNumber = phoneNumber;
			user.AboutAuthor = aboutAuthor;
			if (Photo != null)
				user.PhotoUrl = ImgHelper.UploadSinglePhoto(Photo, _env);

			var result = await _userManager.UpdateAsync(user);

			if (result.Succeeded)
			{
				// User update succeeded
				if (User.IsInRole("Admin"))
				{
					return RedirectToAction(nameof(Index));
				}
				else
				{
					return RedirectToAction("Index", "Dashboard");
				}
			}
			else
			{
				// User update failed
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}

				return View(user);
			}
		}
	}


 }
