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
using CoreBridge.Models.lib;
using System.Text.RegularExpressions;
using XAct;
using System.Collections;

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
        private readonly RoleManager<IdentityRole> _roleManager;

        //リセット用
        public bool IsReset { get; set; } = false;

        public AppManagementUserController(IAppUserService appUserService, ITitleInfoService titleInfoService, SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager, IUserStore<IdentityUser> userStore, IStringLocalizer<SharedResource> stringLocalizer, RoleManager<IdentityRole> roleManager)
        {
            _appUserService = appUserService;
            _titleInfoService = titleInfoService;
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _localizer = stringLocalizer;
            _roleManager = roleManager;
        }

        //[Area("AppManagementUser")]
        //[ActionName("top")]
        public async Task<IActionResult> Index(string? titleCode)
        {
            return View();
        }

        [AuthorizeRoles(AdminUserRoleEnum.AdminUser, AdminUserRoleEnum.BneManager, AdminUserRoleEnum.Manager,
            AdminUserRoleEnum.Reference, AdminUserRoleEnum.EditReference)]
        public async Task<IActionResult> UserList(string Title, AppUserSearchDto appUserSearchDto, string Search, string Clear, MUserIndexViewModel mUserVM)
        {
            //それぞれのデータを読み込み
            bool loginFrst = false;
            if (HttpContext.Request.Cookies.TryGetValue("TitleCode-Cookie", out var value))
            {
                Title = value;
                HttpContext.Response.Cookies.Delete("TitleCode-Cookie");
            }

            var appUserDto = new AppUserDto();
            UserOperation userOperation = new UserOperation(_appUserService);
            appUserDto.UserId = await userOperation.CreateUserId();
            appUserDto.TitleCode = Title;

            var appUserDtos = await _appUserService.ListAsync();
            //List<AppUserDto> userList = new List<AppUserDto>();
            List<AppUserDto> userList = appUserDtos.Where(d => d.IsDelete == false).ToList(); //基本的に削除されてものは表示されなくていいはず（運用でカバーするということなので）

            if (Search != null)
            {
                //検索SQL
                if(appUserSearchDto.UserId != null)
                {
                    userList = userList.Where(x => x.UserId == appUserSearchDto.UserId).ToList();
                }

                if (mUserVM.RoleSearchIndex > 0)
                {
                    userList = userList.Where(x => x.Role == (AdminUserRoleEnum)mUserVM.RoleSearchIndex).ToList();
                }

                if (appUserSearchDto.TitleCode != null)
                {
                    userList = userList.Where(x => x.TitleCode == appUserSearchDto.TitleCode).ToList();
                }

                if (appUserSearchDto.CreatedDate != null && appUserSearchDto.UpdatedDate != null)
                {
                    userList = userList.Where(x => x.CreatedAt >= appUserSearchDto.CreatedDate)
                        .Where(x => x.UpdatedAt <= appUserSearchDto.UpdatedDate).ToList();
                }
            }

            var mUserIndexVM = new MUserIndexViewModel
            {
                AppUserDto = appUserDto,
                AppUserDtos = userList                
            };

            return View(mUserIndexVM);
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
        public async Task<IActionResult> Edit(string id, MUserIndexViewModel mUserIndexViewModel)
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

            //ロール情報を設定
            appUser.Role = (AdminUserRoleEnum)mUserIndexViewModel.RoleIndex;

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
                    if (info == null)
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

                return RedirectToAction("UserList");
            }

            return View(appUser);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(MUserIndexViewModel dto)
        {
            ViewBag.Alert = null;
            //ModelState.Clear();

            //タイトルコードの存在は確認する.
            if (dto.AppUserDto.TitleCode != null)
            {
                //タイトルコードの存在は確認する.
                var check = await _titleInfoService.FindTitleCode(dto.AppUserDto.TitleCode);
                if (check == false)
                {
                    string errorMsg = "同一のタイトルコードがあませんでした！登録済みタイトルコードを利用してください。";
                    ViewBag.Alert = errorMsg;
                    ModelState.AddModelError(string.Empty, errorMsg);
                }
            }

            //パスワードの手動でのバリテーション
            if (dto.AppUserDto.Password != null)
            {
                var hasNumber = new Regex(@"[0-9]+");
                var hasUpperChar = new Regex(@"[A-Z]+");
                var hasMinimum8Chars = new Regex(@".{8,}");
                var hasOtherChar = new Regex(@"[^a-zA-Z0-9]+");

                var isValidated = hasNumber.IsMatch(dto.AppUserDto.Password) && hasUpperChar.IsMatch(dto.AppUserDto.Password)
                    && hasMinimum8Chars.IsMatch(dto.AppUserDto.Password) && hasOtherChar.IsMatch(dto.AppUserDto.Password);
                if (isValidated == false)
                {
                    //エラーにする
                    string errorMsg = "パスワードに使われている文字列が正しくありません！8文字以上の大文字・小文字・数値・数値以外の値が入ったパスワードを使用してください";
                    ViewBag.Alert = errorMsg;

                    ModelState.AddModelError(string.Empty, errorMsg);
                }
            }

            if (dto.AppUserDto.Password != dto.AppUserDto.ConfirmPassword)
            {
                //エラーメッセージ
                string errorMsg = "パスワードが一致しませんでした。一致するパスワードを使用してください";
                ViewBag.Alert = errorMsg;

                ModelState.AddModelError(string.Empty, errorMsg);
            }

            var emailCheck = await _userManager.FindByEmailAsync(dto.AppUserDto.Email);
            if (emailCheck != null)
            {
                string errorMsg = "登録済みのEmailアドレスです。未登録のEmailを利用してください。";
                ViewBag.Alert = errorMsg;
                ModelState.AddModelError(string.Empty, errorMsg);
            }

            //ロール情報の追加(Viewとの連動)
            dto.AppUserDto.Role = (AdminUserRoleEnum)dto.RoleIndex;

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MUserIndexViewModel dto)
        {
            string returnUrl = Url.Content("~/AppUserRegister");

            if (ModelState.IsValid)
            {
                var user = CreateUser();
                await _userStore.SetUserNameAsync(user, dto.AppUserDto.UserId, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, dto.AppUserDto.Password);
                if (result.Succeeded)
                {
                    //_logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var userEmail = await _userManager.SetEmailAsync(user, dto.AppUserDto.Email);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    //ロール管理追加.
                    var roleName = dto.AppUserDto.Role.ToString();
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                    var role = await _userManager.AddToRoleAsync(user, dto.AppUserDto.Role.ToString());

                    //パスワードをハッシュ化
                    var passwordHash = _userManager.PasswordHasher.HashPassword(user, dto.AppUserDto.Password);
                    dto.AppUserDto.Password = passwordHash;

                    //DB登録
                    await _appUserService.GenerateAppUser(dto.AppUserDto);
                    return View(dto);
                }

                ViewBag.Alert = "登録に失敗しました";
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

            return RedirectToAction("UserList");
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
