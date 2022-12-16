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
using Google.Api;
using CoreBridge.Models.lib;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace CoreBridge.Controllers
{
    //ユーザーアカウントに関する制御コントローラーにするつもり

    public class AccountsController : Controller
    {
        private readonly CoreBridgeContext _context;
        private readonly IAppUserService _appUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountsController> _logger;

        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountsController(CoreBridgeContext coreBridgeContext, IAppUserService appUserService, SignInManager<IdentityUser> signInManager, ILogger<AccountsController> logger)
        {
            _context = coreBridgeContext;
            _appUserService = appUserService;
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
            return View();
        }

        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _appUserService == null)
            {
                return NotFound();
            }

            var appUser = await _appUserService.GetByIdAsync(id);
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
        //public async Task<IActionResult> Create()
        //{
        //    AppUser appUser = new();
        //    UserOperation userManager = new(_appUserService);

        //    appUser.UserId = await userManager.CreateUserId();
        //    if(appUser.UserId == String.Empty)
        //    {
        //        return View();
        //    }
        //    return View(appUser);
        //}

        //// POST: Accounts/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("UserId,TitleCode,Password")] AppUserDto appUser)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //タイトルコードの重複は防ぐ.
        //        if (appUser.TitleCode != null)
        //        {
        //            var check = await _appUserService.FindTitleCode(appUser.TitleCode);
        //            if (check == false)
        //            {
        //                //エラーメッセージ
        //                ViewBag.Alert = "同一のタイトルコードがありました！一意のものを使用してください";
        //                return View(_mapper.Map<AppUser>(appUser));
        //            }
        //        }

        //        //登録する
        //        await _appUserService.AddAsync(appUser);
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(appUser);
        //}



        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _appUserService == null)
            {
                return NotFound();
            }

            var appUser = await _appUserService.GetByIdAsync(id);
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

            var appUser = await _appUserService.GetByIdAsync(id);
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
            var appUser = await _appUserService.GetByIdAsync(id);
            if (appUser != null)
            {
                await _appUserService.DeleteAsync(appUser);
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
    }
}
