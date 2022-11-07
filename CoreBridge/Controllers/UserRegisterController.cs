using CoreBridge.Models.DTO;
using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoreBridge.Controllers
{
    public class UserRegisterController : Controller
    {
        private readonly ILogger<UserRegisterController> _logger;
        private readonly IAdminUserService _adminUserService;

        public UserRegisterController(ILogger<UserRegisterController> logger, IAdminUserService adminUserService)
        {
            _logger = logger;
            _adminUserService = adminUserService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Confirm(AdminUserDto dto)
        {

            if (dto.IsValid())
            {
                return View(dto);
            }
            else
            {
                return View("/");
            }
        }

        [HttpPost]
        public IActionResult Create(AdminUserDto dto)
        {

            if (dto.IsValid())
            {
                _adminUserService.GenerateAdminUser(dto);
                return View(dto);
            }
            else
            {
                return View("/");
            }
        }
    }
}
