using AutoMapper;
using CoreBridge.Models.DTO;
using CoreBridge.Models.Entity;
using CoreBridge.Models.lib;
using CoreBridge.Services;
using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace CoreBridge.Controllers
{
    public class AppUserRegisterController : Controller
    {
        private readonly IAppUserService _appUserService;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly ILogger<UserLoginController> _logger;

        public AppUserRegisterController(IAppUserService appUserSerice, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager, IUserStore<IdentityUser> userStore, ILogger<UserLoginController> logger)
        {
            _appUserService = appUserSerice;
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            AppUserDto appUser = new();
            UserOperation userManager = new(_appUserService);

            appUser.UserId = await userManager.CreateUserId();
            if (appUser.UserId == String.Empty)
            {
                return View();
            }
            return View(appUser);
            //return View();
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(AppUserDto dto)
        {
            string returnUrl = Url.Content("~/");

            if (ModelState.IsValid)
            {
                //タイトルコードの重複は防ぐ.
                if (dto.TitleCode != null)
                {
                    var check = await _appUserService.FindTitleCode(dto.TitleCode);
                    if (check == false)
                    {
                        //エラーメッセージ
                        ViewBag.Alert = "同一のタイトルコードがありました！一意のものを使用してください";
                        //return RedirectToPage("/", dto);
                        //return RedirectToPage("/AppUserRegister", new { userId = dto.UserId, returnUrl = returnUrl });
                        return View(dto);
                    }
                }

                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, dto.UserId, CancellationToken.None);
                //await _emailStore.SetEmailAsync(user, dto.EMail, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, dto.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(dto.EMail, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    //{
                    //    return RedirectToPage("RegisterUserLoginConfirmation", new { userId = dto.UserId, returnUrl = returnUrl });
                    //}
                    //else
                    //{
                    //    await _signInManager.SignInAsync(user, isPersistent: false);
                    //    return LocalRedirect(returnUrl);
                    //}

                    return View(dto);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppUserDto dto)
        {

            if (dto.IsValid())
            {
                await _appUserService.GenerateAdminUser(dto);
                return View(dto);
            }
            else
            {
                return View("/");
            }
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

    }
}
