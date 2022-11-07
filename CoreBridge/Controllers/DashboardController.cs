using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoreBridge.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly IAdminUserService _adminUserService;

        public DashboardController(ILogger<DashboardController> logger, IAdminUserService adminUserService)
        {
            _logger = logger;
            _adminUserService = adminUserService;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
