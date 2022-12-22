using Microsoft.AspNetCore.Mvc;
using CoreBridge.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using CoreBridge.Models.DTO;
using CoreBridge.Services.Interfaces;
using CoreBridge.ViewModels;
using CoreBridge.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using CoreBridge.Attributes;
using CoreBridge.Models;

namespace CoreBridge.Controllers
{
    //[Route("{titleCode}/[controller]/[action]")]
    public class AppManagementUserController : Controller
    {
        private readonly IAppUserService _appUserService;
        private readonly ITitleInfoService _titleInfoService;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly ILogger<UserLoginController> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AppManagementUserController(IAppUserService appUserService, ITitleInfoService titleInfoService, SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager, IUserStore<IdentityUser> userStore, IStringLocalizer<SharedResource> stringLocalizer)
        {
            _appUserService = appUserService;
            _titleInfoService = titleInfoService;
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _localizer = stringLocalizer;
        }

        //[Area("AppManagementUser")]
        //[ActionName("top")]
        public async Task<IActionResult> Index(string? titleCode)
        {
            return View();
        }

        [AuthorizeRoles(AdminUserRoleEnum.AdminUser, AdminUserRoleEnum.BneManager, AdminUserRoleEnum.Manager,
            AdminUserRoleEnum.Reference, AdminUserRoleEnum.EditReference)]
        public async Task<IActionResult> UserList(string Title)
        {
            return View(await _appUserService.FindAsync());
        }

        // GET: Accounts/Create
        public async Task<IActionResult> Create()
        {
            //ここではゲーム管理ユーザー用の登録画面へ移動させる.
            return LocalRedirect("/AppUserRegister");
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,TitleCode,Password")] AppUserDto appUser)
        {
            if (ModelState.IsValid)
            {
                ////タイトルコードの重複は防ぐ.
                var check = await _titleInfoService.FindTitleCode(appUser.TitleCode);
                if (check == false)
                {
                    string errorMsg = "同一のタイトルコードがあませんでした！登録済みタイトルコードを利用してください。";
                    ViewBag.Alert = errorMsg;
                    ModelState.AddModelError(string.Empty, errorMsg);
                    return View();
                }

                //登録する
                await _appUserService.GenerateAppUser(appUser);
                return View();
            }
            return View(appUser);
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

        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _appUserService == null)
            {
                return NotFound();
            }

            var appUser = await _appUserService.GetByUserIdAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }
            return View(appUser);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserId,TitleCode,Password")] AppUserDto appUser)
        {
            //if (id != appUser.UserId)
            //{
            //    return NotFound();
            //}

            if (ModelState.IsValid)
            {
                try
                {
                    await _appUserService.UpdateAsync(appUser);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppUserExists(appUser.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                //return LocalRedirect("/Accounts/UserList");
                return View();
            }

            return View(appUser);
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
                        string errorMsg = "同一のタイトルコードがありました！一意のものを使用してください";
                        ViewBag.Alert = errorMsg;
                        //return RedirectToPage("/", dto);
                        //return RedirectToPage("/AppUserRegister", new { userId = dto.UserId, returnUrl = returnUrl });
                        //return View(dto);

                        ModelState.AddModelError(string.Empty, errorMsg);
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

                    return View(dto);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(dto);
        }

        // GET: Accounts/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _appUserService == null)
            {
                return NotFound();
            }

            var appUser = await _appUserService.GetByUserIdAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_appUserService == null)
            {
                return Problem("Entity set 'CoreBridgeContext.AppUsers'  is null.");
            }
            var appUser = await _appUserService.GetByUserIdAsync(id);
            if (appUser != null)
            {
                await _appUserService.DeleteAsync(appUser);
            }

            return View();
        }

        private bool AppUserExists(string id)
        {
            var titleinfo = _appUserService.GetByIdAsync(id);
            if (titleinfo == null)
            {
                return false;
            }

            return true;
        }

        public async Task<IActionResult> Signout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "AppManagementUser");
        }
    }
}
