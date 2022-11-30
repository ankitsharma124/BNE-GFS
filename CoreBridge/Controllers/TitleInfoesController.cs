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
using Google.Api;
using CoreBridge.Models.DTO;
using AutoMapper;
using CoreBridge.Models.Repositories;

namespace CoreBridge.Controllers
{
    public class TitleInfoesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITitleInfoService _titleInfoService;

        public TitleInfoesController(IUnitOfWork unitOfWork, ITitleInfoService titleInfoService)
        {
            _unitOfWork = unitOfWork;
            _titleInfoService = titleInfoService;
        }

        // GET: TitleInfoes
        public async Task<IActionResult> Index()
        {
            return View(await _unitOfWork.TitleInfoRepository.ListAsync());
            //return View(await _titleInfoService.ListAsync());
        }

        // GET: TitleInfoes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _titleInfoService == null)
            {
                return NotFound();
            }

            var titleInfo = await _titleInfoService.GetByIdAsync(id);
            if (titleInfo == null)
            {
                return NotFound();
            }

            return View(titleInfo);
        }

        // GET: TitleInfoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TitleInfoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TitleName,TitleCode,TrialTitleCode,Ptype,SwitchAppId,XboxTitleId,PsClientId,PsClientSecoret,SteamAppId,SteamPublisherKey,DevUrl,TestUrl,QaUrl,ProdUrl")] TitleInfoDto titleInfo)
        {
            if (ModelState.IsValid)
            {
                var check = await _titleInfoService.AddAsync(titleInfo);
                if (check == null)
                {
                    ViewBag.Alert = "同一のタイトルコードかトライアルコードがありました！一意のものを使用してください";
                    return View();
                }

                return RedirectToAction(nameof(Index));
            }

            return View(titleInfo);
        }

        // GET: TitleInfoes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _titleInfoService == null)
            {
                return NotFound();
            }

            var titleInfo = await _titleInfoService.GetByIdAsync(id);
            if (titleInfo == null)
            {
                return NotFound();
            }
            return View(titleInfo);
        }

        // POST: TitleInfoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("TitleName,TitleCode,TrialTitleCode,Ptype,SwitchAppId,XboxTitleId,PsClientId,PsClientSecoret,SteamAppId,SteamPublisherKey,DevUrl,TestUrl,QaUrl,ProdUrl")] TitleInfoDto titleInfo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _titleInfoService.UpdateAsync(titleInfo);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TitleInfoExists(id))
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
            return View(titleInfo);
        }

        // GET: TitleInfoes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _titleInfoService == null)
            {
                return NotFound();
            }

            var titleInfo = await _titleInfoService.GetByIdAsync(id);
            if (titleInfo == null)
            {
                return NotFound();
            }

            return View(titleInfo);
        }

        // POST: TitleInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_titleInfoService == null)
            {
                return Problem("Entity set 'CoreBridgeContext.TitleInfo'  is null.");
            }

            var titleInfo = await _titleInfoService.GetByIdAsync(id);
            if (titleInfo != null)
            {
                await _titleInfoService.DeleteAsync(titleInfo);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TitleInfoExists(string id)
        {
            var titleinfo = _unitOfWork.TitleInfoRepository.GetByIdAsync(id);
            if (titleinfo == null)
            {
                return false;
            }

            return true;
        }
    }
}
