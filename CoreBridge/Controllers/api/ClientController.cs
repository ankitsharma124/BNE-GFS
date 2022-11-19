using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace CoreBridge.Controllers.api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ClientController : ClientServerControllerBase, IDisposable
    {
        public ClientController(IHostEnvironment env, IResponseService responseService, IDistributedCache cache,
           IConfiguration configService, ILoggerService loggerService) : base(env, responseService, cache, configService, loggerService)
        {
        }

        #region BaseController class methods
        protected override string GetUserInfoKey() { return ""; }
        protected override string GetSessionKey() { return ""; }
        protected override void SessionCheck()
        {
        }

        protected override void SessionUpdate()
        {
        }
        #endregion
    }
}
