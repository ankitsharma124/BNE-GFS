using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoreBridge.Models.Context;
using CoreBridge.Models.Entity;
using CoreBridge.Models.Interfaces;
using CoreBridge.Services.Interfaces;
using CoreBridge.Models.DTO;
using CoreBridge.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Authentication;
using System.Text;
using System.Text.Encodings.Web;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using CoreBridge.Models;
using CoreBridge.Attributes;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using XAct.Users;

namespace CoreBridge.Controllers
{
    /// <summary>
    /// スーパーユーザー向け画面コントローラー
    /// ゲーム制作ユーザーの初期登録、ゲーム制作ユーザーアカウントの一覧表示等を制御
    /// </summary>
    [AuthorizeRoles(AdminUserRoleEnum.AdminUser)]
    public class AccountsController : Controller
    {
        private readonly IAppUserService _appUserService;
        private readonly ITitleInfoService _titleInfoService;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountsController> _logger;

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountsController(IAppUserService appUserService, ITitleInfoService titleInfoService,
            SignInManager<IdentityUser> signInManager,UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<AccountsController> logger)
        {
            _appUserService = appUserService;
            _titleInfoService = titleInfoService;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;

            var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AppUserDto, AppUser>());
            _mapper = new Mapper(mapConfig);
            _logger = logger;
        }

        // GET: Accounts        
        public async Task<IActionResult> Index()
        {
            //一覧表示.
            return View(await _appUserService.FindAsync());
        }

        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(string id)
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

        public async Task<IActionResult> UserList()
        {
            return View(await _appUserService.FindAsync());
        }

        // GET: Accounts/Create
        public async Task<IActionResult> Create()
        {
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
                //タイトルコードの確認をする
                if (appUser.TitleCode != null)
                {
                    var check = await _titleInfoService.FindTitleCode(appUser.TitleCode);
                    if (check == false)
                    {
                        string errorMsg = "同一のタイトルコードがあませんでした！登録済みタイトルコードを利用してください。";
                        ViewBag.Alert = errorMsg;
                        ModelState.AddModelError(string.Empty, errorMsg);
                        return View();
                    }
                }

                //登録する
                await _appUserService.GenerateAppUser(appUser);
                return View();
            }
            return View(appUser);
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
        public async Task<IActionResult> Edit(string id, [Bind("UserId,TitleCode,Role,Email,Password")] AppUserDto appUser)
        {
            if (ModelState.IsValid)
            {
                //エラーチェック
                var check = await _titleInfoService.FindTitleCode(appUser.TitleCode);
                if (check == false)
                {
                    string errorMsg = "同一のタイトルコードがあませんでした！登録済みタイトルコードを利用してください。";
                    ViewBag.Alert = errorMsg;
                    ModelState.AddModelError(string.Empty, errorMsg);
                    return View(appUser);
                }

                try
                {
                    //サインイン情報のアップデート

                    //EmailからUserインスタンス取得
                    var user = await _userManager.FindByEmailAsync(appUser.Email);

                    //ロール管理アップデート.
                    var roleName = appUser.Role.ToString();
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                    await _userManager.AddToRoleAsync(user, roleName);

                    //UserManagerのパスワード更新
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var ret = await _userManager.ResetPasswordAsync(user, token, appUser.Password);

                    //パスワードをハッシュ化
                    var new_passwordHash = _userManager.PasswordHasher.HashPassword(user, appUser.Password);
                    appUser.Password = new_passwordHash;

                    //DB更新
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

                return RedirectToAction(nameof(Index));
            }

            return View(appUser);
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

            //return RedirectToAction(nameof(Index));
            return LocalRedirect("/Accounts/UserList");
            //return View();
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

        // Sign In View
        [AllowAnonymous]
        public async Task<IActionResult> SignIn()
        {
            return View();
        }

        public async Task<IActionResult> Signout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
