﻿using CoreBridge.Models.DTO;
using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoreBridge.Controllers
{
    public class AppUserLoginController : Controller
    {
        private readonly IAppUserService _appUserSerice;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly ILogger<UserLoginController> _logger;

        public AppUserLoginController(IAppUserService appUserSerice, SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager, IUserStore<IdentityUser> userStore, ILogger<UserLoginController> logger)
        {
            _appUserSerice = appUserSerice;
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(AppUserDto dto)
        {
            var info = await _appUserSerice.GetByUserIdAsync(dto.UserId);
            //UserID確認
            if(info == null)
            {
                ModelState.AddModelError(string.Empty, "正しいUserIDを指定してください。");
                return View(dto);
            }

            //タイトルコード確認
            if (dto.TitleCode != info.TitleCode)
            {
                ModelState.AddModelError(string.Empty, "タイトルコードが不正です。");
                return View(dto);
            }

            string returnUrl = Url.Content("~/");
            if (ModelState.IsValid)
            {
                //サインイン処理
                var result = await _signInManager.PasswordSignInAsync(dto.UserId, dto.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    if (dto.TitleCode != null)
                    {
                        CookieOptions cookieOptions = new CookieOptions();
                        cookieOptions.Secure = true;
                        cookieOptions.Expires = DateTime.UtcNow.AddDays(7);
                        HttpContext.Response.Cookies.Append("TitleCode-Cookie", dto.TitleCode, cookieOptions);
                    }

                    return LocalRedirect($"/AppManagementUser/UserList"); 
                }
                if (result.RequiresTwoFactor) //二段階認証
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = false });
                }
                if (result.IsLockedOut) //ログアウト確認
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "E-Mailまたは、パスワードが不正です");
                    return View(dto);
                }
            }

            return View(dto);
        }
    }
}
