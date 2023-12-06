﻿using BizNewsAppDB1.DataServer;
using BizNewsAppDB1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BizNewsAppDB1.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    public class ProfileUserController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;

        public ProfileUserController(AppDBContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            User user = await _userManager.FindByIdAsync(userId);

            ViewData["User"] = user;

            return View(user);
           
        }


        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                User user = await _userManager.FindByIdAsync(userId);

                var uploadsFolder = Path.Combine(_env.WebRootPath, "/uploads/");
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                user.PhotoUrl = "/uploads/" + fileName;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    User user = await _userManager.FindByIdAsync(userId);

                    // Geçici bir kopya oluştur
                    User tempUser = new User
                    {
                        Firstname = user.Firstname,
                        Lastname = user.Lastname,
                        AboutAuthor = user.AboutAuthor,
                        Email = user.Email
                    };

                    // Yeni bilgileri güncelle
                    user.Firstname = model.Firstname;
                    user.Lastname = model.Lastname;
                    user.AboutAuthor = model.AboutAuthor;
                    user.Email = model.Email;

                    // Veritabanına güncelleme işlemi
                    var updateResult = await _userManager.UpdateAsync(user);

                    if (updateResult.Succeeded)
                    {
                        // Geçici kopyayı gerçek veritabanına kaydet
                        var saveTempResult = await _userManager.UpdateAsync(tempUser);

                        if (!saveTempResult.Succeeded)
                        {
                            // Geçici kopyayı kaydetme işlemi başarısız oldu
                            // Hata işleme kodu buraya eklenmeli
                        }

                        // Başarılı güncelleme durumunda, ilgili sayfaya yönlendir
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in updateResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", "Profil güncellenirken bir hata oluştu.");
                }
            }

            // Hata durumunda veya başarısızlık durumunda, aynı sayfaya model ile dön
            return View("Index", model);
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string renewPassword)
        {
            if (newPassword != renewPassword)
            {
                ModelState.AddModelError("", "New password and confirmation password do not match.");
                return RedirectToAction("Index");
            }

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            User user = await _userManager.FindByIdAsync(userId);

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View("Index", user);
        }
    }
}
