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

namespace CoreBridge.Controllers
{
    public class AccountsController : Controller
    {
        private readonly CoreBridgeContext _context;
        private readonly IAppUserService _appUserService;

        public AccountsController(CoreBridgeContext coreBridgeContext, IAppUserService appUserService)
        {
            _context = coreBridgeContext;
            _appUserService = appUserService;
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            return View(await _appUserService.FindAsync());
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

        // GET: Accounts/Create
        public IActionResult Create()
        {
            return View();
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
                //_context.Add(appUser);
                //await _context.SaveChangesAsync();
                await _appUserService.AddAsync(appUser);
                return RedirectToAction(nameof(Index));
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
