using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreBridge.Controllers.api
{
    public abstract class BaseController : ControllerBase, IDisposable
    {





        [NonAction]
        public void Dispose()
        {
            //todo: transactions?
        }
    }
}
