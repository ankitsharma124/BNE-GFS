using CoreBridge.Models.DTO;
using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoreBridge.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IAdminUserService _adminUserService;

        // サインイン必須
        private readonly SignInManager<IdentityUser> _signInManager;

        public LoginController(ILogger<LoginController> logger, IAdminUserService adminUserService, SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _adminUserService = adminUserService;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Home", "Index");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(AdminUserDto dto)
        {
            //サインインの処理を書く

            //AdminUserDto result = await _adminUserService.LoginAdminUser(dto);
            //if (result == null)
            ////if(false)
            //{
            //    ModelState.AddModelError(string.Empty, "E-Mailまたは、パスワードが不正です");
            //    return View(dto);
            //}
            //else
            //{
            //    return LocalRedirect("/Dashboard");
            //}

            string returnUrl = Url.Content("~/");

            var result = await _signInManager.PasswordSignInAsync(dto.EMail, dto.Password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return LocalRedirect("/Dashboard");
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
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(dto);
            }
        }
    }
}
