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

        [ViewData]
        public string IsDeleteTab { get; set; } = "";

        public bool IsDeleteView { get; set; } = false;

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
        public async Task<IActionResult> Index(bool IsDeleteView = false)
        {
            if (IsDeleteView)
            {
                //すべて表示する
                ViewData["IsDeleteTab"] = "checked";
                return View(await _appUserService.FindAsync());
            }

            ViewData["IsDeleteTab"] = "";
            var getInfo = await _appUserService.FindAsync();
            var viewInfo = getInfo.Where(d => d.IsDelete == false);
            return View(viewInfo);
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

            appUser.Password = String.Empty;

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
                ViewBag.Alert = null;
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
                    //DBに保存されている情報を取得
                    var info = await _appUserService.GetByUserIdAsync(appUser.UserId);
                    if(info == null) 
                    {
                        return View(appUser); 
                    }

                    //EmailからUserインスタンス取得
                    var user = await _userManager.FindByEmailAsync(info.Email);

                    //Emailの変更確認.
                    if (info.Email != appUser.Email)
                    {
                        //トークン生成
                        var email_token = await _userManager.GenerateChangeEmailTokenAsync(user, appUser.Email);

                        //ユーザーのEmail情報を更新
                        await _userManager.ChangeEmailAsync(user, appUser.Email, email_token);
                    }

                    //サインイン情報のアップデート
                    if (appUser.Password != null)
                    {
                        //UserManagerのパスワード更新
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var ret = await _userManager.ResetPasswordAsync(user, token, appUser.Password);

                        //パスワードをハッシュ化
                        var new_passwordHash = _userManager.PasswordHasher.HashPassword(user, appUser.Password);
                        appUser.Password = new_passwordHash;
                    }

                    //ロール管理アップデート.
                    var roleName = appUser.Role.ToString();
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                    await _userManager.AddToRoleAsync(user, roleName);

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
                //削除フラグをON！
                appUser.IsDelete = true;

                //DB更新
                await _appUserService.UpdateAsync(appUser);

                /* Spannerの場合リレーショナルを組んでいると削除がロジックでやろうとすると難航する可能性あり
                 * 今回は下記の処理は行わず、削除フラグにて表示を切り替えるようにする
                 * 削除する必要がある場合は、運用でカバーする流れにする（ 12/26 根本さんと相談済み：野竹）

                //EmailからUserインスタンス取得
                var user = await _userManager.FindByEmailAsync(appUser.Email);

                //ロール情報の削除 <- 情報を取ってこれない.
                var role = await _roleManager.FindByIdAsync(user.Id);
                await _roleManager.DeleteAsync(role);

                //ユーザー情報の削除
                var deluser = await _userManager.FindByIdAsync(user.Id);
                await _userManager.DeleteAsync(deluser);

                //ユーザーDBの削除
                await _appUserService.DeleteAsync(appUser);
                */
            }

            return RedirectToAction(nameof(Index));
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
