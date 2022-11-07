using CoreBridge.Services.Interfaces;
using MessagePack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CoreBridge.Controllers.api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminUserController : ControllerBase
    {
        private readonly ILogger<AdminUserController> _logger;
        private readonly IAdminUserService _adminUserService;

        public AdminUserController(ILogger<AdminUserController> logger, IAdminUserService adminUserService)
        {
            _logger = logger;
            _adminUserService = adminUserService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var list = await _adminUserService.ListAsync();
            byte[] data = MessagePackSerializer.Serialize(list);
            
            // TODO：JSonでのResponse
            // return new JsonResult(list);
            
            // TODO：MessagePack でのResponse
            return new ObjectResult(list);
        }

    }
}
