#if DEBUG

using CoreBridge.Models.DTO.Requests;
using CoreBridge.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace CoreBridge.Controllers.api.server
{

    [Route("api/server/[controller]/[action]")]
    public class ServerTestController : ServerController
    {
        private readonly IRequestService _reqService;
        public ServerTestController(IHostEnvironment env, IResponseService responseService, IDistributedCache cache,
          IConfiguration configService, ILogger loggerService, ITitleInfoService titleInfoService,
          IRequestService reqService)
            : base(env, responseService, cache, configService, loggerService, titleInfoService)
        {

        }


        [HttpPost]
        public IActionResult MiddlewareTest([FromBody] TestParam testParam)
        {
            //var header = _reqService.GetDebugBodyCopyInBytesFromHeader(Request);
            //Debug.WriteLine(header);

            return new JsonResult("OK");
        }
    }
}
#endif