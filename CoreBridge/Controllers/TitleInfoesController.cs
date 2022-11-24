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

namespace CoreBridge.Controllers
{
    public class TitleInfoesController : Controller
    {
        //private readonly CoreBridgeContext _context;
        private readonly IUnitOfWork _unitOfWork;

        //public TitleInfoesController(CoreBridgeContext context)
        //{
        //    _context = context;
        //}

        public TitleInfoesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: TitleInfoes
        public async Task<IActionResult> Index()
        {
            //return View(await _context.TitleInfo.ToListAsync());
            return View(await _unitOfWork.TitleInfoRepository.ListAsync());
        }

        // GET: TitleInfoes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            //if (id == null || _unitOfWork.TitleInfo == null)
            if (id == null || _unitOfWork.TitleInfoRepository == null)
            {
                return NotFound();
            }

            //var titleInfo = await _context.TitleInfo
            //.FirstOrDefaultAsync(m => m.TitleCode == id);
            var titleInfo = await _unitOfWork.TitleInfoRepository.GetByIdAsync(id);
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
        public async Task<IActionResult> Create([Bind("TitleName,TitleCode,TrialTitleCode,Ptype,SwitchAppId,XboxTitleId,PsClientId,PsClientSecoret,SteamAppId,SteamPublisherKey,DevUrl,QaUrl,ProdUrl,Id,CreatedAt,UpdatedAt")] TitleInfo titleInfo)
        {
            if (ModelState.IsValid)
            {
                //_context.Add(titleInfo);
                //await _context.SaveChangesAsync();
                await _unitOfWork.TitleInfoRepository.AddAsync(titleInfo);
                await _unitOfWork.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(titleInfo);
        }

        // GET: TitleInfoes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            //if (id == null || _context.TitleInfo == null)
            //{
            //    return NotFound();
            //}

            //var titleInfo = await _context.TitleInfo.FindAsync(id);
            if (id == null || _unitOfWork.TitleInfoRepository == null)
            {
                return NotFound();
            }

            var titleInfo = await _unitOfWork.TitleInfoRepository.GetByIdAsync(id);
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
        public async Task<IActionResult> Edit(string id, [Bind("TitleName,TitleCode,TrialTitleCode,Ptype,SwitchAppId,XboxTitleId,PsClientId,PsClientSecoret,SteamAppId,SteamPublisherKey,DevUrl,QaUrl,ProdUrl")] TitleInfo titleInfo)
        {
            if (id != titleInfo.TitleCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(titleInfo);
                    //await _context.SaveChangesAsync();
                    await _unitOfWork.TitleInfoRepository.UpdateAsync(titleInfo);
                    await _unitOfWork.CommitAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TitleInfoExists(titleInfo.TitleCode))
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
            //if (id == null || _context.TitleInfo == null)
            //{
            //    return NotFound();
            //}

            //var titleInfo = await _context.TitleInfo
            //    .FirstOrDefaultAsync(m => m.TitleCode == id);
            if (id == null || _unitOfWork.TitleInfoRepository == null)
            {
                return NotFound();
            }

            var titleInfo = await _unitOfWork.TitleInfoRepository.GetByIdAsync(id);
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
            //if (_context.TitleInfo == null)
            //{
            //    return Problem("Entity set 'CoreBridgeContext.TitleInfo'  is null.");
            //}
            //var titleInfo = await _context.TitleInfo.FindAsync(id);
            //if (titleInfo != null)
            //{
            //    _context.TitleInfo.Remove(titleInfo);
            //}

            //await _context.SaveChangesAsync();
            if (_unitOfWork.TitleInfoRepository == null)
            {
                return Problem("Entity set 'CoreBridgeContext.TitleInfo'  is null.");
            }
            var titleInfo = await _unitOfWork.TitleInfoRepository.GetByIdAsync(id);
            if (titleInfo != null)
            {
                await _unitOfWork.TitleInfoRepository.DeleteAsync(titleInfo);
            }

            //await _context.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        private  bool TitleInfoExists(string id)
        {
            //return _context.TitleInfo.Any(e => e.TitleCode == id);
            var titleinfo = _unitOfWork.TitleInfoRepository.GetByIdAsync(id);
            if(titleinfo == null)
            {
                return false;
            }

            return true;
        }
    }
}
