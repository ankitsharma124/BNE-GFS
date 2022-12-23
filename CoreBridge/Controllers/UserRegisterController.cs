using CoreBridge.Models.DTO;
using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using CoreBridge.Models;
using XAct.Users;


namespace CoreBridge.Controllers
{
    public class UserRegisterController : Controller
    {
        private readonly IAdminUserService _adminUserService;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<UserRegisterController> _logger;
        private readonly IEmailSender _emailSender;

        public UserRegisterController(ILogger<UserRegisterController> logger, IAdminUserService adminUserService, UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IUserStore<IdentityUser> userStore, IEmailSender emailSender)
        {
            _logger = logger;
            _adminUserService = adminUserService;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View();
        }

        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public bool DisplayConfirmAccountLink { get; set; }
        public string EmailConfirmationUrl { get; set; }

        [HttpPost]
        public async Task<IActionResult> Confirm(AdminUserDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.EMail);
            if (user != null)
            {
                ViewBag.Alert = "既に利用されているEmailアドレスです！";
                ModelState.AddModelError(string.Empty, "既に利用されているEmailアドレスです！");
                return View(dto);
            }

            if (dto.IsValid())
            {
                return View(dto);
            }
            else
            {
                return View("/");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminUserDto dto)
        {
            string returnUrl = Url.Content("~/");

            if (ModelState.IsValid)
            {
                //サインインのための処理
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, dto.EMail, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, dto.EMail, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    //ロール追加(強制的にAdminUserのみ)
                    await _roleManager.CreateAsync(new IdentityRole(AdminUserRoleEnum.AdminUser.ToString()));
                    await _userManager.AddToRoleAsync(user, AdminUserRoleEnum.AdminUser.ToString());

                    //Email設定
                    await _userManager.SetEmailAsync(user, dto.EMail);

                    //パスワードをハッシュ化
                    var passwordHash = _userManager.PasswordHasher.HashPassword(user, dto.Password);
                    dto.Password = passwordHash;

                    //DB登録
                    await _adminUserService.GenerateAdminUser(dto);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = dto.EMail, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(dto);
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
