using CoreBridge.Services.Interfaces;
using MessagePack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace CoreBridge.Controllers.api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminUserController : BaseControllerForMsgPack
    {
        private readonly ILogger<AdminUserController> _logger;
        private readonly IAdminUserService _adminUserService;

        public AdminUserController(ILogger<AdminUserController> logger, IAdminUserService adminUserService,
            IWebHostEnvironment env) : base(env)
        {
            _logger = logger;
            _adminUserService = adminUserService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var list = await _adminUserService.ListAsync();
            return ReturnMsgPack(list);
        }

    }
}
