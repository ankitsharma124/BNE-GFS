using Microsoft.AspNetCore.Mvc;
using CoreBridge.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using CoreBridge.Models.DTO;
using CoreBridge.Services.Interfaces;
using CoreBridge.ViewModels;

namespace CoreBridge.Controllers
{
    public class AppManagementUserController : Controller
    {
        public IActionResult Index()
        {
            var AppUser = new AppUser();


            return View();
        }
    }
}
