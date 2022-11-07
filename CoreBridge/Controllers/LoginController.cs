using CoreBridge.Models.DTO;
using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoreBridge.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IAdminUserService _adminUserService;

        public LoginController(ILogger<LoginController> logger, IAdminUserService adminUserService)
        {
            _logger = logger;
            _adminUserService = adminUserService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Home", "Index");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(AdminUserDto dto)
        {
            AdminUserDto result = await _adminUserService.LoginAdminUser(dto);
            if (result == null)
            //if(false)
            {
                ModelState.AddModelError(string.Empty, "E-Mailまたは、パスワードが不正です");
                return View(dto);
            }
            else
            {
                return LocalRedirect("/Dashboard");
            }
        }
    }
}
