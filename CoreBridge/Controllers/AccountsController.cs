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

namespace CoreBridge.Controllers
{
    //ユーザーアカウントに関する制御コントローラーにするつもり

    public class AccountsController : Controller
    {
        private readonly IAppUserService _appUserService;
        private readonly ITitleInfoService _titleInfoService;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountsController> _logger;

        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountsController(IAppUserService appUserService, ITitleInfoService titleInfoService,
            SignInManager<IdentityUser> signInManager, ILogger<AccountsController> logger)
        {
            _appUserService = appUserService;
            _titleInfoService = titleInfoService;
            _signInManager = signInManager;

            var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<AppUserDto, AppUser>());
            _mapper = new Mapper(mapConfig);
            _logger = logger;
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            //一覧表示.
            //表示されるのは管理ユーザー or 一般ユーザーになる！？
            //return View(await _appUserService.FindAsync());
            //return RedirectToAction("UserLogin", "index");
            //return View();
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

                    //var check = await _appUserService.FindTitleCode(appUser.TitleCode);
                    //if (check == false)
                    //{
                    //    //エラーメッセージ
                    //    string errorMsg = "同一のタイトルコードがありました！一意のものを使用してください";
                    //    ViewBag.Alert = errorMsg;
                    //    ModelState.AddModelError(string.Empty, errorMsg);
                    //    return View();
                    //}
                }

                //登録する
                await _appUserService.GenerateAppUser(appUser);
                return View();
            }
            return View(appUser);
        }



        // GET: Accounts/Edit/5
        //[AuthorizeRoles(AdminUserRoleEnum.AdminUser)]
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

                return RedirectToAction(nameof(Index));
                //return LocalRedirect("/Accounts/UserList");
                //return View();
            }
            //return View(appUser);
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
            //return LocalRedirect("/Accounts/UserList");
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
    }
}
