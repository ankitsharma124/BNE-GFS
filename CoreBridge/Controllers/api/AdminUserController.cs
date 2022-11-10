using CoreBridge.Models;
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
        private readonly ILoggerService _logger;
        private readonly IAdminUserService _adminUserService;

        public AdminUserController(ILoggerService logger, IAdminUserService adminUserService,
            IWebHostEnvironment env) : base(env)
        {
            _logger = logger;
            _adminUserService = adminUserService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var list = await _adminUserService.ListAsync();
                return ReturnMsgPack(list);
            }
            catch (CoreBridgeException ex)
            {
                _logger.LogError("GetAdminUser", ex);
                return StatusCode(ex.StatusCode); //ToDo: check
            }
        }

    }
}
