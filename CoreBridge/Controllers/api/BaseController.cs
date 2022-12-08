using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XAct.State;

namespace CoreBridge.Controllers.api
{
    public class BaseController : ControllerBase
    {
        private readonly ISessionStateService _sss;
        public BaseController(ISessionStateService sss)
        {
        }


    }
}
