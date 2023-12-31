﻿using AutoMapper;
using CoreBridge.Models;
using CoreBridge.Models.DTO;
using CoreBridge.Models.Entity;
using CoreBridge.Models.lib;
using CoreBridge.Services;
using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace CoreBridge.Controllers
{

    public class AppUserRegisterController : Controller
    {
        private readonly IAppUserService _appUserService;
        private readonly ITitleInfoService _titleInfoService;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly ILogger<UserLoginController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AppUserRegisterController(IAppUserService appUserSerice, ITitleInfoService titleInfoService, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager, IUserStore<IdentityUser> userStore, ILogger<UserLoginController> logger, RoleManager<IdentityRole> roleManager)
        {
            _appUserService = appUserSerice;
            _titleInfoService = titleInfoService;
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _logger = logger;
            _roleManager = roleManager;
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
            ViewBag.Alert = null;

            //タイトルコードの存在は確認する.
            if (dto.TitleCode != null)
            {
                //タイトルコードの存在は確認する.
                var check = await _titleInfoService.FindTitleCode(dto.TitleCode);
                if (check == false)
                {
                    string errorMsg = "同一のタイトルコードがあませんでした！登録済みタイトルコードを利用してください。";
                    ViewBag.Alert = errorMsg;
                    ModelState.AddModelError(string.Empty, errorMsg);
                }
            }

            //パスワードの手動でのバリテーション
            if(dto.Password != null)
            {
                var hasNumber = new Regex(@"[0-9]+");
                var hasUpperChar = new Regex(@"[A-Z]+");
                var hasMinimum8Chars = new Regex(@".{8,}");
                var hasOtherChar= new Regex(@"[^a-zA-Z0-9]+");

                var isValidated = hasNumber.IsMatch(dto.Password) && hasUpperChar.IsMatch(dto.Password)
                    && hasMinimum8Chars.IsMatch(dto.Password) && hasOtherChar.IsMatch(dto.Password);
                if (isValidated == false)
                {
                    //エラーにする
                    string errorMsg = "パスワードに使われている文字列が正しくありません！8文字以上の大文字・小文字・数値・数値以外の値が入ったパスワードを使用してください";
                    ViewBag.Alert = errorMsg;

                    ModelState.AddModelError(string.Empty, errorMsg);
                }
            }

            if (dto.Password != dto.ConfirmPassword)
            {
                //エラーメッセージ
                string errorMsg = "パスワードが一致しませんでした。一致するパスワードを使用してください";
                ViewBag.Alert = errorMsg;

                ModelState.AddModelError(string.Empty, errorMsg);
            }

            var emailCheck = await _userManager.FindByEmailAsync(dto.Email);
            if (emailCheck != null)
            {
                string errorMsg = "登録済みのEmailアドレスです。未登録のEmailを利用してください。";
                ViewBag.Alert = errorMsg;
                ModelState.AddModelError(string.Empty, errorMsg);
            }

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppUserDto dto)
        {
            string returnUrl = Url.Content("~/AppUserRegister");

            if (ModelState.IsValid)
            {
                var user = CreateUser();
                await _userStore.SetUserNameAsync(user, dto.UserId, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var userEmail = await _userManager.SetEmailAsync(user, dto.Email);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    //ロール管理追加.
                    await CheckAppRole(dto.Role.ToString());
                    var role = await _userManager.AddToRoleAsync(user, dto.Role.ToString());

                    //パスワードをハッシュ化
                    var passwordHash = _userManager.PasswordHasher.HashPassword(user, dto.Password);
                    dto.Password = passwordHash;

                    //DB登録
                    await _appUserService.GenerateAppUser(dto);
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

        /// <summary>
        /// ロール管理
        /// </summary>
        private async Task CheckAppRole(string roleName)
        {
            IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                //成功
            }
        }

    }
}
