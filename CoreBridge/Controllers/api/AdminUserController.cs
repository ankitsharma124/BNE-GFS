using CoreBridge.Models;
using CoreBridge.Models.Exceptions;
using CoreBridge.Services;
using CoreBridge.Services.Interfaces;
using MessagePack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace CoreBridge.Controllers.api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminUserController : ControllerBase
    {
        private readonly ILogger<AdminUserService> _logger;
        private readonly IAdminUserService _adminUserService;

        public AdminUserController(ILogger<AdminUserService> logger, IAdminUserService adminUserService)
        {
            _logger = logger;
            _adminUserService = adminUserService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var list = await _adminUserService.ListAsync();

            return new JsonResult(list);
        }

    }
}
