using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BizNewsAppDB1.Areas.Admin.Controllers
{
	[Area(nameof(Admin))]	
	public class RoleController : Controller
	{
		private readonly RoleManager<IdentityRole> _roleManager;

		public RoleController(RoleManager<IdentityRole> roleManager)
		{
			_roleManager = roleManager;
		}

		public IActionResult Index()
		{
			var Role = _roleManager.Roles.ToList();
			return View(Role);
		}


		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> Create(IdentityRole IdentityRole)
		{

			if (IdentityRole.Name == null)
			{

				ViewBag.Error = "Empty value cannot be sent!";
                return View();
			}


			var Checkrole = await _roleManager.FindByNameAsync(IdentityRole.Name);


			if (Checkrole != null)
			{
				ViewBag.Errors = "The role is exsist!!";
				return View();
			}

			if (!ModelState.IsValid)
			{
				ModelState.AddModelError("error", "Role name is exsis!!!");
				return View();
			}
			else
			{
				await _roleManager.CreateAsync(IdentityRole);
				return RedirectToAction("Index");
			}


		}
		[HttpGet]
		public IActionResult Delete()
		{
			return View();
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(IdentityRole IdentityRole)
		{

			await _roleManager.DeleteAsync(IdentityRole);
			return RedirectToAction("Index");

		}
		public async Task<IActionResult> Detail(string id)
		{
			if (id == null) return NotFound();
			var tags = _roleManager.Roles.FirstOrDefault(x => x.Id == id);
			if (tags == null) return NotFound();
			return View(tags);
		}


		public async Task<IActionResult> UpdateRole(string Id)
		{
			if (Id is null)
			{
				ModelState.AddModelError("Errror", "something went wrong");
				return View();
			}

            IdentityRole role = await _roleManager.FindByIdAsync(Id);

            if (role == null)
            {
                ModelState.AddModelError("Error", "Role Id or Name is Not found!");
                return View();
            }

            return View(role);
        }
	}
}
